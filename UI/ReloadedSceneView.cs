using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CameraToggleReloaded.Configuration;
using CameraToggleReloaded.Helper;
using HMUI;
using IPA.Utilities.Async;
using JetBrains.Annotations;
using SiraUtil.Logging;
using Zenject;

namespace CameraToggleReloaded.UI
{
    [HotReload(RelativePathToLayout = @"Views\ReloadedSceneView.bsml")]
    [ViewDefinition("CameraToggleReloaded.UI.Views.ReloadedSceneView.bsml")]
    internal class ReloadedSceneView : BSMLAutomaticViewController
    {
        private static ReloadedConfig _reloadedConfig = null!;
        private SiraLog _siraLog = null!;
        private ReloadedMenuView _reloadedMenuView = null!;

        [Inject]
        [UsedImplicitly]
        private void Construct(ReloadedConfig reloadedConfig, SiraLog siraLog, ReloadedMenuView reloadedMenuView)
        {
            _reloadedConfig = reloadedConfig;
            _siraLog = siraLog;
            _reloadedMenuView = reloadedMenuView;

            _reloadedMenuView.ReloadedSceneView = this;
        }

        [UIComponent("ctrScenesList")]
        public CustomCellListTableData? sceneList;

        [UsedImplicitly]
        [UIValue("ctrScenes")] 
        internal List<object> AvailableSettings { get; } = new List<object>();

        [UIAction("scene-select")]
        internal void SceneSelect(TableView _, object obj)
        {
            var reloadedSceneEntry = (ReloadedSceneEntry)obj;
            var existing = _reloadedConfig.Scenes.FirstOrDefault(x => x.Scene == reloadedSceneEntry.Scene);
            if (existing == null)
            {
                // this should never happen
                existing = new ReloadedSceneSetting
                {
                    Scene = reloadedSceneEntry.Scene,
                };
                _reloadedConfig.Scenes.Add(existing);
            }

            _reloadedMenuView.CurrentSetting = existing;
        }
        
        [UIValue("auto-switch")]
        public bool AutoSwitchFromCustomScene
        {
            get => ReloadedHelper.IsAutoSwitchActivated;
            set => ReloadedHelper.IsAutoSwitchActivated = value;
        }

        [UIAction("re-sync")]
        public void UpdateList(ReloadedSceneSetting? currentSetting = null)
        {
            if (sceneList == null)
            {
                return;
            }

            sceneList.tableView.ClearSelection();
            AvailableSettings.Clear();

            _reloadedConfig.Scenes = ReloadedHelper.GetAvailableCustomScenes()
                .OrderBy(x => x.Key)
                .Select(x =>
                {
                    var exists = _reloadedConfig.Scenes.FirstOrDefault(y => y.Scene == x.Key);
                    return exists ?? new ReloadedSceneSetting
                    {
                        Scene = x.Key,
                        Command = x.Key.ToLower().Replace(' ', '_'),
                        //Reward = "???",
                    };
                })
                .ToList();

            if (_reloadedConfig.Scenes.Count == 0)
            {
                // this should never happen...
                _siraLog.Critical("no scenes found...");
                AvailableSettings.Add(new ReloadedSceneEntry("~ERROR~", false));
            }
            else
            {
                AvailableSettings.AddRange(_reloadedConfig.Scenes
                    .Select(x => new ReloadedSceneEntry(x.Scene, x.IsEnabled))
                    .OrderByDescending(x => x.WhiteListed)
                    .ThenBy(x => x.Scene)
                    .ToList()
                );
            }
            
            // preset data
            var index = currentSetting == null ? 0 : _reloadedConfig.Scenes.IndexOf(currentSetting);
            _reloadedMenuView.CurrentSetting = _reloadedConfig.Scenes[index];
            
            UnityMainThreadTaskScheduler.Factory.StartNew(() =>
            {
                sceneList.tableView.ReloadData();
                sceneList.tableView.ScrollToCellWithIdx(index, TableView.ScrollPositionType.Beginning, false);
            }).ConfigureAwait(false);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            UpdateList();
        }
    }
}