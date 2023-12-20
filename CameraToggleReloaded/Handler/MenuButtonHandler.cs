using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using CameraToggleReloaded.UI;
using JetBrains.Annotations;
using Zenject;

namespace CameraToggleReloaded.Handler
{
    [UsedImplicitly]
    internal class MenuButtonHandler : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly ReloadedFlowCoordinator _flowCoordinator;

        public MenuButtonHandler(MainFlowCoordinator mainFlowCoordinator, ReloadedFlowCoordinator reloadedFlowCoordinator)
        {
            _menuButton = new MenuButton("CameraToggle Reloaded", "It is finally back", MenuButtonClick);
            _mainFlowCoordinator = mainFlowCoordinator;
            _flowCoordinator = reloadedFlowCoordinator;
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }

        private void MenuButtonClick()
        {
            _mainFlowCoordinator.PresentFlowCoordinator(_flowCoordinator);
        }
    }
}