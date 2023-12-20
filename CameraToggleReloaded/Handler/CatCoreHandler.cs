using System;
using System.Collections.Generic;
using System.Linq;
using Camera2.Configuration;
using CameraToggleReloaded.Configuration;
using CameraToggleReloaded.Helper;
using CatCore;
using CatCore.Models.Twitch.PubSub.Responses.ChannelPointsChannelV1;
using CatCore.Services.Multiplexer;
using CatCore.Services.Twitch.Interfaces;
using JetBrains.Annotations;
using SiraUtil.Logging;
using UnityEngine.SceneManagement;
using Zenject;

namespace CameraToggleReloaded.Handler
{
    [UsedImplicitly]
    internal class CatCoreHandler : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly ReloadedConfig _reloadedConfig;
        private readonly CatCoreInstance _instance;
        private readonly Dictionary<string, DateTime> _cooldown;
        private DateTime _globalCooldown;
        private object? _multiplexer;
        private ITwitchService? _twitchService;
        private ITwitchPubSubServiceManager? _twitchPubSubServiceManager;
        /*
        private Tuple<MultiplexedMessage, string?>? _lateToggle;
        private bool _lateToggleFlag;
        */
        
        public CatCoreHandler(SiraLog siraLog, ReloadedConfig reloadedConfig)
        {
            _siraLog = siraLog;
            _reloadedConfig = reloadedConfig;
            _instance = CatCoreInstance.Create();
            _cooldown = new Dictionary<string, DateTime>();
            _globalCooldown = DateTime.Now;
        }

        private ChatServiceMultiplexer? ChatServiceMultiplexer
        {
            get
            {
                if (_multiplexer == null)
                {
                    return null;
                }

                return (ChatServiceMultiplexer)_multiplexer;
            }
            set => _multiplexer = value;
        }

        public void Initialize()
        {
            ChatServiceMultiplexer = _instance.RunAllServices();
            ChatServiceMultiplexer!.OnTextMessageReceived += OnTextMessageReceived;
            _twitchService = ChatServiceMultiplexer.GetTwitchPlatformService();
            _twitchPubSubServiceManager = _twitchService.GetPubSubService();
            _twitchPubSubServiceManager.OnRewardRedeemed += TwitchPubSubServiceManagerOnOnRewardRedeemed;
            
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        public void Dispose()
        {
            if (ChatServiceMultiplexer != null)
            {
                ChatServiceMultiplexer!.OnTextMessageReceived -= OnTextMessageReceived;
                ChatServiceMultiplexer = null;
            }

            if (_twitchPubSubServiceManager != null)
            {
                _twitchPubSubServiceManager!.OnRewardRedeemed -= TwitchPubSubServiceManagerOnOnRewardRedeemed;
                _twitchPubSubServiceManager = null;
            }

            _instance.StopAllServices();
        }

        private void OnTextMessageReceived(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            if (!message.Message.ToLower().StartsWith(_reloadedConfig.CommandInitiator))
            {
                return;
            }
            
            if (!_reloadedConfig.Enabled)
            {
                SendMessage(message.Channel, "CameraToggle Reloaded is currently not enabled");
                return;
            }

            if (IsInCoolDown(message.Sender.UserName, message.Sender.IsModerator || message.Sender.IsBroadcaster))
            {
                SendMessage(message.Channel, _reloadedConfig.CooldownIsGlobal
                    ? "toggle has to cool down"
                    : $"@{message.Sender.DisplayName} you have to cool down :3"
                );
                return;
            }

            var cmd = message.Message.Split(' ');
            if (cmd.Length < 2)
            {
                SendMessage(message.Channel, $"what do you want me to do @{message.Sender.DisplayName}? try to use list command");
                return;
            }

            var lowerCommand = cmd[1].ToLower();
            //_siraLog.Debug(lowerCommand);
            switch (lowerCommand)
            {
                /*
                case "help":
                    // ??
                    SendMessage(message.Channel, "why? just why?", true);
                    break;
                */
                case ReloadedHelper.DefaultSceneName:
                    SwitchToScene(message, ReloadedHelper.DefaultSceneName);
                    break;

                case "list":
                    // list all available scenes
                    if (_reloadedConfig.Scenes.Count > 0)
                    {
                        SendMessage(message.Channel, "available scenes: " + string.Join(", ", _reloadedConfig.Scenes.Where(x => x.IsEnabled).Select(x => x.Command).Prepend(ReloadedHelper.DefaultSceneName)));
                    }
                    else
                    {
                        SendMessage(message.Channel, "there are no scenes available");
                    }

                    break;

                default:
                    SwitchToScene(message, lowerCommand);
                    break;
            }
        }

        private void TwitchPubSubServiceManagerOnOnRewardRedeemed(string whatever, RewardRedeemedData redeemedData)
        {
            _siraLog.Debug(whatever);
            _siraLog.Debug(redeemedData.User.DisplayName);
            _siraLog.Debug(redeemedData.Reward);
        }

        private void SendMessage(MultiplexedChannel channel, string message, bool forceSend = false)
        {
            if (!forceSend && !_reloadedConfig.SendInfos)
            {
                return;
            }

            try
            {
                if (_reloadedConfig.AddPreventChar && _reloadedConfig.PreventChar.Length > 0)
                {
                    message = _reloadedConfig.PreventChar + message;
                }
                channel.SendMessage(message);
            }
            catch (Exception e)
            {
                _siraLog.Error(e);
            }
        }

        private bool IsInCoolDown(string username, bool isMod)
        {
            if (_reloadedConfig.ModsCanBypassCooldown && isMod)
            {
                return false;
            }
            if (_reloadedConfig.CooldownIsGlobal)
            {
                return _globalCooldown > DateTime.Now;
            }

            if (!_cooldown.ContainsKey(username))
            {
                _cooldown.Add(username, DateTime.Now.AddSeconds(-_reloadedConfig.Cooldown * 2));
            }
            
            return _cooldown[username] > DateTime.Now;
        }

        private void SwitchToScene(MultiplexedMessage message, string? name, bool wasLate = false)
        {
            // add fake scene for reverting back to default
            var tmpScenes = _reloadedConfig.Scenes.ToList();
            tmpScenes.Add(new ReloadedSceneSetting
            {
                Command = ReloadedHelper.DefaultSceneName,
                UseCommand = true,
                Scene = ReloadedHelper.DefaultSceneName,
                // why tf does Enum.GetValues throw errors?
                //ValidForSceneTypes = Enum.GetValues(typeof(SceneTypes)).ToList()
                ValidForSceneTypes = new List<SceneTypes>
                {
                    SceneTypes.Menu,
                    SceneTypes.MultiplayerMenu,
                    SceneTypes.Playing,
                    SceneTypes.Playing360,
                    SceneTypes.PlayingModmap,
                    SceneTypes.PlayingMulti,
                    SceneTypes.SpectatingMulti,
                    SceneTypes.Replay,
                    SceneTypes.FPFC
                }
            });

            // look for scene
            var scene = tmpScenes.FirstOrDefault(x => x.Command.ToLower() == name);
            if (scene == null)
            {
                SendMessage(message.Channel, $"unable to find scene {name}");
                return;
            }
            
            if (ReloadedHelper.CurrentScene == scene.Scene)
            {
                SendMessage(message.Channel, "already using this scene");
                return;
            }
            
            if (!IsInCorrectMode(scene))
            {
                SendMessage(message.Channel, $"scene {scene.Scene} is not valid for current game mode");
                return;
            }
            /*
            if (_lateToggleFlag)
            {
                SendMessage(message.Channel, "There is already a toggle queued");
                return;
            }
            */
            //SendMessage(message.Channel, "CameraToggle queued :3");
            SetCooldown(message, wasLate);

            ReloadedHelper.SwitchToSceneSave(scene.Scene);
        }

        private bool IsInCorrectMode(ReloadedSceneSetting sceneSetting)
        {
            // default always works
            if (sceneSetting.Scene == ReloadedHelper.DefaultSceneName)
            {
                return true;
            }
            
            // check if in correct scene type
            var current = ReloadedHelper.CurrentSceneType;
            if (sceneSetting.ValidForSceneTypes.Contains(current))
            {
                return true;
            }
            /*
            _lateToggle = new Tuple<MultiplexedMessage, string?>(message, name);
            _lateToggleFlag = true;
            */
            return false;
        }

        private void SetCooldown(MultiplexedMessage message, bool wasLate)
        {
            SendMessage(message.Channel, wasLate ? "Late CameraToggle fired :3" : "CameraToggle queued :3");

            var nextPossibleTrigger = DateTime.Now.AddSeconds(_reloadedConfig.Cooldown);
            _cooldown[message.Sender.UserName] = nextPossibleTrigger;
            _globalCooldown = nextPossibleTrigger;
        }
        
        private void SceneManagerOnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            /*
            _siraLog.Debug(newScene.name);
            if (newScene.name == "EmptyTransition" || newScene.name == "ShaderWarmup" || newScene.name == "ShaderWarmup")
            {
                _currentGameType = ReloadedGameType.Menu;
                return;
            }

            _currentGameType = newScene.name == "GameCore" ? ReloadedGameType.Solo : ReloadedGameType.Menu;
            if (_currentGameType == ReloadedGameType.Menu && !ReloadedHelper.MenuSceneNames.Contains(newScene.name))
            {
                _currentGameType = ReloadedGameType.Multi;
            }
            */
        }
        
        private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            if (scene.name == "EmptyTransition")
            {
                return;
            }
            _siraLog.Debug(scene.name);
            /*
            if (_currentGameType == ReloadedGameType.Menu || !_lateToggleSet || _lateToggle == null)
            {
                return;
            }

            UnityMainThreadTaskScheduler.Factory.StartNew(() =>
            {
                SwitchToScene(_lateToggle.Item1, _lateToggle.Item2, true);
                _lateToggle = null;
                _lateToggleSet = false;
            }).ConfigureAwait(false);
            */
        }
    }
}