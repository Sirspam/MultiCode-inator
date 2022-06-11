using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using MultiCode_inator.Configuration;
using MultiCode_inator.Utils;
using Zenject;

namespace MultiCode_inator.UI.ViewControllers
{
    internal class CommandToggleViewController : IInitializable, IDisposable
    {
        private readonly PluginConfig _pluginConfig;
        private readonly GameplaySetupViewController _gameplaySetupViewController;
        private readonly MultiplayerSettingsPanelController _multiplayerSettingsPanelController;

        public CommandToggleViewController(PluginConfig pluginConfig, GameplaySetupViewController gameplaySetupViewController, MultiplayerSettingsPanelController multiplayerSettingsPanelController)
        {
            _pluginConfig = pluginConfig;
            _gameplaySetupViewController = gameplaySetupViewController;
            _multiplayerSettingsPanelController = multiplayerSettingsPanelController;
        }

        [UIValue("command-enabled")]
        private bool CommandEnabled
        {
            get => StaticFields.PluginEnabled && _pluginConfig.CommandEnabled;
            set => _pluginConfig.CommandEnabled = value;
        }

        [UIValue("dependency-installed")] 
        private bool DependencyInstalled => StaticFields.PluginEnabled;
        
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
            if (firstActivation)
            {
                BSMLParser.instance.Parse("<toggle-setting value=\"command-enabled\" text=\"Enable MultiCode Command\" interactable=\"~dependency-installed\" apply-on-change=\"true\" bind-value=\"true\"/>", _multiplayerSettingsPanelController.gameObject, this);
                if (!DependencyInstalled) // Text is a teeny bit jank, not too much of an issue and I can't be bothered to try and fix it
                {
                    BSMLParser.instance.Parse($"<text text=\"{StaticFields.NoDependenciesMessage}\" font-align=\"Center\" color=\"red\"/>", _multiplayerSettingsPanelController.gameObject, this);
                }
            }
        }
    }
}