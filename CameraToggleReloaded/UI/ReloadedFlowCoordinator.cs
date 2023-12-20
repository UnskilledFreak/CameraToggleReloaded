using BeatSaberMarkupLanguage;
using HMUI;
using JetBrains.Annotations;
using Zenject;

namespace CameraToggleReloaded.UI
{
    internal class ReloadedFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlowCoordinator = null!;
        private ReloadedMenuView _reloadedMenuView = null!;
        private ReloadedSceneView _reloadedSceneView = null!;

        [Inject]
        [UsedImplicitly]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, ReloadedMenuView reloadedMenuView, ReloadedSceneView reloadedSceneView)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _reloadedMenuView = reloadedMenuView;
            _reloadedSceneView = reloadedSceneView;
        }
        
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation)
            {
                return;
            }

            SetTitle("CameraToggleReloaded");
            showBackButton = true;
            
            //_sceneView.UpdateList();
            
            ProvideInitialViewControllers(_reloadedMenuView, _reloadedSceneView);
        }

        protected override void BackButtonWasPressed(ViewController _)
        {
            _mainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}