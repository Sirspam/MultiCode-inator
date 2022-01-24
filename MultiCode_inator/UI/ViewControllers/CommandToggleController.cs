using System;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using TheMultiCode_inator.Configuration;
using Zenject;

namespace TheMultiCode_inator.UI.ViewControllers
{
    internal class CommandToggleController : IInitializable, IDisposable
    {
        private readonly PluginConfig _pluginConfig;
        private readonly GameplaySetupViewController _gameplaySetupViewController;
        private readonly MultiplayerSettingsPanelController _multiplayerSettingsPanelController;

        public CommandToggleController(PluginConfig pluginConfig, GameplaySetupViewController gameplaySetupViewController, MultiplayerSettingsPanelController multiplayerSettingsPanelController)
        {
            _pluginConfig = pluginConfig;
            _gameplaySetupViewController = gameplaySetupViewController;
            _multiplayerSettingsPanelController = multiplayerSettingsPanelController;
        }

        [UIValue("command-enabled")]
        private bool CommandEnabled
        {
            get => _pluginConfig.CommandEnabled;
            set => _pluginConfig.CommandEnabled = value;
        }

        public void Initialize()
        {
            _gameplaySetupViewController.didActivateEvent += GameplaySetupViewController_didActivateEvent;
        }

        public void Dispose()
        {
            _gameplaySetupViewController.didActivateEvent -= GameplaySetupViewController_didActivateEvent;
        }

        private void GameplaySetupViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation) BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "MultiCode_inator.UI.Views.CommandToggle.bsml"), _multiplayerSettingsPanelController.gameObject, this);
        }
    }
}