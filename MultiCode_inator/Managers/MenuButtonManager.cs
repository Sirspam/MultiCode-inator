using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using MultiCode_inator.UI.FlowCoordinators;
using Zenject;

namespace MultiCode_inator.Managers
{
	internal class MenuButtonManager : IInitializable, IDisposable
	{
		private readonly MenuButton _menuButton;
		private readonly MainFlowCoordinator _mainFlowCoordinator;
		private readonly MultiCodeFlowCoordinator _multiCodeFlowCoordinator;

		public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, MultiCodeFlowCoordinator multiCodeFlowCoordinator)
		{
			_menuButton = new MenuButton("MultiCode-inator", MenuButtonClicked);
			_mainFlowCoordinator = mainFlowCoordinator;
			_multiCodeFlowCoordinator = multiCodeFlowCoordinator;
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

		private void MenuButtonClicked()
		{
			_mainFlowCoordinator.PresentFlowCoordinator(_multiCodeFlowCoordinator);
		}
	}
}