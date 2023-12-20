using CameraToggleReloaded.Handler;
using CameraToggleReloaded.UI;
using JetBrains.Annotations;
using Zenject;

namespace CameraToggleReloaded.Installers
{
    [UsedImplicitly]
    internal class ReloadedMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuButtonHandler>().AsSingle();
            Container.Bind<ReloadedMenuView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ReloadedSceneView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ReloadedFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
        }
    }
}