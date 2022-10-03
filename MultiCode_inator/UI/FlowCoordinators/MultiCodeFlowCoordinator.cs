using BeatSaberMarkupLanguage;
using HMUI;
using MultiCode_inator.UI.ViewControllers;
using Zenject;

namespace MultiCode_inator.UI.FlowCoordinators
{
	internal class MultiCodeFlowCoordinator : FlowCoordinator
	{
		private MainFlowCoordinator _mainFlowCoordinator = null!;
		private MultiCodeSettingsViewController _multiCodeSettingsViewController = null!;

		[Inject]
		private void Construct(MainFlowCoordinator mainFlowCoordinator, MultiCodeSettingsViewController multiCodeSettingsViewController)
		{
			_mainFlowCoordinator = mainFlowCoordinator;
			_multiCodeSettingsViewController = multiCodeSettingsViewController;
		}
		
		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			SetTitle("MultiCode-inator");
			showBackButton = true;
			
			ProvideInitialViewControllers(_multiCodeSettingsViewController);
		}

		protected override void BackButtonWasPressed(ViewController topViewController)
		{
			_mainFlowCoordinator.DismissFlowCoordinator(this);
		}
	}
}