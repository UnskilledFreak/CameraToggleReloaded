using CameraToggleReloaded.Configuration;
using CameraToggleReloaded.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using IPA.Logging;
using JetBrains.Annotations;
using SiraUtil.Zenject;

namespace CameraToggleReloaded
{
    [Plugin(RuntimeOptions.DynamicInit)]
    [NoEnableDisable]
    [UsedImplicitly]
    public class Plugin
    {
        internal static string Version { get; private set; } = null!;
        
        [Init]
        [UsedImplicitly]
        public void Init(Config config, PluginMetadata metadata, Logger logger, Zenjector zenjector)
        {
            Version = metadata.HVersion.ToString();
            
            zenjector.UseLogger(logger);
            zenjector.UseMetadataBinder<Plugin>();
            
            zenjector.Install<ReloadedAppInstaller>(Location.App, logger, config.Generated<ReloadedConfig>());
            zenjector.Install<ReloadedMenuInstaller>(Location.Menu);
        }
    }
}