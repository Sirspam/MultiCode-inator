using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Reflection;
using TheMultiCode_inator.Configuration;
using Zenject;

namespace TheMultiCode_inator.UI.ViewControllers
{
    internal class CommandToggleController : IInitializable, IDisposable
    {
        private readonly GameplaySetupViewController gameplaySetupViewController;
        private readonly MultiplayerSettingsPanelController multiplayerSettingsPanelController;

        public CommandToggleController(GameplaySetupViewController gameplaySetupViewController, MultiplayerSettingsPanelController multiplayerSettingsPanelController)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
            this.multiplayerSettingsPanelController = multiplayerSettingsPanelController;
        }

        [UIValue("command-enabled")]
        private bool CommandEnabled
        { 
            get => PluginConfig.Instance.CommandEnabled;
            set => PluginConfig.Instance.CommandEnabled = value;
        }

        public void Initialize() => gameplaySetupViewController.didActivateEvent += GameplaySetupViewController_didActivateEvent;

        public void Dispose() => gameplaySetupViewController.didActivateEvent -= GameplaySetupViewController_didActivateEvent;

        private void GameplaySetupViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "TheMultiCode_inator.UI.Views.CommandToggle.bsml"), multiplayerSettingsPanelController.gameObject, this);
            }
        }
    }
}