using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using Camera2.Configuration;
using CameraToggleReloaded.Configuration;
using CameraToggleReloaded.Helper;
using JetBrains.Annotations;
using Zenject;

namespace CameraToggleReloaded.UI
{
    [HotReload(RelativePathToLayout = "ReloadedMenuView.bsml")]
    [ViewDefinition("CameraToggleReloaded.UI.Views.ReloadedMenuView.bsml")]
    internal class ReloadedMenuView : BSMLAutomaticViewController
    {
        private ReloadedConfig _reloadedConfig = null!;
        private ReloadedSceneSetting _reloadedSceneSetting = new ReloadedSceneSetting();

        public ReloadedSceneSetting CurrentSetting
        {
            get => _reloadedSceneSetting;
            set
            {
                _reloadedSceneSetting = value;

                NotifyPropertyChanged(nameof(SceneName));
                NotifyPropertyChanged(nameof(SceneUseCommand));
                NotifyPropertyChanged(nameof(SceneCommand));
                //NotifyPropertyChanged(nameof(SceneUseReward));
                //NotifyPropertyChanged(nameof(SceneReward));
                NotifyPropertyChanged(nameof(SceneForMenu));
                NotifyPropertyChanged(nameof(SceneForMultiplayerMenu));
                NotifyPropertyChanged(nameof(SceneForPlaying));
                NotifyPropertyChanged(nameof(SceneForPlaying360));
                NotifyPropertyChanged(nameof(SceneForPlayingModMap));
                NotifyPropertyChanged(nameof(SceneForPlayingMulti));
                NotifyPropertyChanged(nameof(SceneForSpectatingMulti));
                NotifyPropertyChanged(nameof(SceneForReplay));
            }
        }

        public ReloadedSceneView ReloadedSceneView { get; set; } = null!;

        [Inject]
        public void Construct(ReloadedConfig reloadedConfig)
        {
            _reloadedConfig = reloadedConfig;
        }

        #region general config

        [UIValue("enabled")]
        public bool Enabled
        {
            get => _reloadedConfig.Enabled;
            set => _reloadedConfig.Enabled = value;
        }

        [UIValue("commandInitiator")]
        public string CommandInitiator
        {
            get => _reloadedConfig.CommandInitiator;
            set => _reloadedConfig.CommandInitiator = value.Length < 2 ? "!ctr" : value;
        }

        [UIValue("cooldown")]
        public int Cooldown
        {
            get => _reloadedConfig.Cooldown;
            set => _reloadedConfig.Cooldown = value;
        }

        [UIValue("global-cooldown")]
        public bool GlobalCooldown
        {
            get => _reloadedConfig.CooldownIsGlobal;
            set => _reloadedConfig.CooldownIsGlobal = value;
        }

        [UIValue("modsCanBypassCooldown")]
        public bool ModsCanBypassCooldown
        {
            get => _reloadedConfig.ModsCanBypassCooldown;
            set => _reloadedConfig.ModsCanBypassCooldown = value;
        }

        [UIValue("sendInfos")]
        public bool SendInfos
        {
            get => _reloadedConfig.SendInfos;
            set => _reloadedConfig.SendInfos = value;
        }

        [UIValue("addPreventChar")]
        public bool AddPreventChar
        {
            get => _reloadedConfig.AddPreventChar;
            set => _reloadedConfig.AddPreventChar = value;
        }

        [UIValue("preventChar")]
        public string PreventChar
        {
            get => _reloadedConfig.PreventChar;
            set => _reloadedConfig.PreventChar = value.Length == 0 ? "! " : value;
        }

        #endregion

        #region scene specific config

        [UIValue("scene-use-command")]
        public bool SceneUseCommand
        {
            get => CurrentSetting.UseCommand;
            set
            {
                CurrentSetting.UseCommand = value;
                SaveConfig();
            }
        }

        [UIValue("scene-command")]
        public string SceneCommand
        {
            get => CurrentSetting.Command;
            set
            {
                CurrentSetting.Command = ReloadedHelper.AddSaveCommandOption(_reloadedConfig, value);
                SaveConfig();
            }
        }
        /*
        [UIValue("scene-use-reward")]
        public bool SceneUseReward
        {
            get => CurrentSetting.UseReward;
            set
            {
                CurrentSetting.UseReward = value;
                SaveConfig();
            }
        }
        
        [UIValue("scene-reward")]
        public string SceneReward
        {
            get => CurrentSetting.Reward;
            set
            {
                CurrentSetting.Reward = value;
                SaveConfig();
            }
        }
        */
        [UIValue("scene-for-Menu")]
        public bool SceneForMenu
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.Menu);
            set => SetValueForSetting(value, SceneTypes.Menu);
        }

        [UIValue("scene-for-MultiplayerMenu")]
        public bool SceneForMultiplayerMenu
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.MultiplayerMenu);
            set => SetValueForSetting(value, SceneTypes.MultiplayerMenu);
        }

        [UIValue("scene-for-Playing")]
        public bool SceneForPlaying
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.Playing);
            set => SetValueForSetting(value, SceneTypes.Playing);
        }

        [UIValue("scene-for-Playing360")]
        public bool SceneForPlaying360
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.Playing360);
            set => SetValueForSetting(value, SceneTypes.Playing360);
        }

        [UIValue("scene-for-PlayingModMap")]
        public bool SceneForPlayingModMap
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.PlayingModmap);
            set => SetValueForSetting(value, SceneTypes.PlayingModmap);
        }

        [UIValue("scene-for-PlayingMulti")]
        public bool SceneForPlayingMulti
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.PlayingMulti);
            set => SetValueForSetting(value, SceneTypes.PlayingMulti);
        }

        [UIValue("scene-for-SpectatingMulti")]
        public bool SceneForSpectatingMulti
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.SpectatingMulti);
            set => SetValueForSetting(value, SceneTypes.SpectatingMulti);
        }

        [UIValue("scene-for-Replay")]
        public bool SceneForReplay
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.Replay);
            set => SetValueForSetting(value, SceneTypes.Replay);
        }

        [UIValue("scene-for-FPFC")]
        public bool SceneForFPFC
        {
            get => CurrentSetting.ValidForSceneTypes.Contains(SceneTypes.FPFC);
            set => SetValueForSetting(value, SceneTypes.FPFC);
        }

        [UsedImplicitly]
        [UIValue("scene-name")]
        private string SceneName => $"Selected scene: {CurrentSetting.Scene}";

        [UsedImplicitly]
        [UIValue("copy-thingy")]
        private string CopyThingy => $"CameraToggleReloaded {Plugin.Version} By UnskilledFreak";

        #endregion

        private void SaveConfig()
        {
            NotifyPropertyChanged(nameof(_reloadedConfig.Scenes));
            ReloadedSceneView.UpdateList(CurrentSetting);
        }

        private void SetValueForSetting(bool val, SceneTypes sceneType)
        {
            var exists = CurrentSetting.ValidForSceneTypes.Contains(sceneType);
            if (val && !exists)
            {
                CurrentSetting.ValidForSceneTypes.Add(sceneType);
                SaveConfig();
            }
            else if (!val && exists)
            {
                CurrentSetting.ValidForSceneTypes.Remove(sceneType);
                SaveConfig();
            }
        }
    }
}