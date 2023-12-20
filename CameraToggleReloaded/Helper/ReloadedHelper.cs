using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camera2.Configuration;
using CameraToggleReloaded.Configuration;
using IPA.Loader;
using IPA.Utilities.Async;

namespace CameraToggleReloaded.Helper
{
    internal static class ReloadedHelper
    {
        private static bool? _catCoreInstalled;
        private static bool? _camera2Installed;

        public const string DefaultSceneName = "default";
        public static string CurrentScene { get; private set; } = DefaultSceneName;
        public static readonly HashSet<string> MenuSceneNames = new HashSet<string>
        {
            "MainMenu", 
            "MenuViewCore",
            "MenuCore",
            "MenuViewControllers"
        };
        
        public static IReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAvailableCustomScenes()
        {
            return Camera2.SDK.Scenes.customScenes;
        }

        public static SceneTypes CurrentSceneType => Camera2.SDK.Scenes.current;

        public static bool CatCoreInstalled
        {
            get
            {
                _catCoreInstalled ??= PluginManager.GetPluginFromId("CatCore") != null;
                return (bool)_catCoreInstalled;
            }
        }

        public static bool Camera2Installed
        {
            get
            {
                _camera2Installed ??= PluginManager.GetPluginFromId("Camera2") != null;
                return (bool)_camera2Installed;
            }
        }

        public static string AddSaveCommandOption(ReloadedConfig config, string newCommand)
        {
            var counter = 1;

            string check;
            do
            {
                check = newCommand + (counter == 1 ? "" : counter.ToString());
                counter++;
            } while (
                check.Equals(DefaultSceneName, StringComparison.InvariantCultureIgnoreCase)
                || check.Equals("list", StringComparison.InvariantCultureIgnoreCase)
                || config.Scenes.FirstOrDefault(x => x.Command.ToLower() == check) != null
            );

            return check;
        }

        public static void SwitchToSceneSave(string toSceneName)
        {
            UnityMainThreadTaskScheduler.Factory.StartNew(() =>
            {
                if (toSceneName == DefaultSceneName)
                {
                    CurrentScene = DefaultSceneName;
                    Camera2.SDK.Scenes.ShowNormalScene();
                }
                else
                {
                    CurrentScene = toSceneName;
                    Camera2.SDK.Scenes.SwitchToCustomScene(toSceneName);
                }
            }).ConfigureAwait(false);
        }

        public static bool IsAutoSwitchActivated()
        {
            // load scene json
            // return autoswitchFromCustom value
            return true;
        }
    }
}