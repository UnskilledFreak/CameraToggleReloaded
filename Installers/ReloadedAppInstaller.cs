using CameraToggleReloaded.Configuration;
using CameraToggleReloaded.Handler;
using IPA.Logging;
using JetBrains.Annotations;
using Zenject;

namespace CameraToggleReloaded.Installers
{
    [UsedImplicitly]
    internal class ReloadedAppInstaller : Installer
    {
        private readonly Logger _logger;
        private readonly ReloadedConfig _reloadedConfig;

        public ReloadedAppInstaller(Logger logger, ReloadedConfig reloadedConfig)
        {
            _logger = logger;
            _reloadedConfig = reloadedConfig;
        }
        
        public override void InstallBindings()
        {
            Container.BindInstance(_reloadedConfig).AsSingle();

            if (!Helper.ReloadedHelper.CatCoreInstalled)
            {
                _logger.Critical("CatCore not found");
                return;
            }

            if (!Helper.ReloadedHelper.Camera2Installed)
            {
                _logger.Critical("Camera2 not found");
                return;
            }

            Container.BindInterfacesTo<CatCoreHandler>().AsSingle();
        }
    }
}