using System;
using System.ComponentModel;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using MultiCode_inator.Configuration;
using MultiCode_inator.Managers;
using MultiCode_inator.Utils;
using SiraUtil.Web.SiraSync;
using TMPro;
using Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace MultiCode_inator.UI.ViewControllers
{
    internal class CommandToggleModalViewController : IInitializable, IDisposable
    {
        private bool _parsed;
        
        private Button? _multiCodeButton;
        private Button? _serverCodeButton;
        private ImageView? _multiCodeButtonUnderline;

        [UIParams] 
        private readonly BSMLParserParams _parserParams = null!;
        
        private readonly PluginConfig _pluginConfig;
        private readonly ScreenCanvasManager _screenCanvasManager;
        private readonly TimeTweeningManager _timeTweeningManager;
        private readonly GameplaySetupViewController _gameplaySetupViewController;
        private readonly MultiplayerSettingsPanelController _multiplayerSettingsPanelController;

        public CommandToggleModalViewController(PluginConfig pluginConfig, ScreenCanvasManager screenCanvasManager, TimeTweeningManager timeTweeningManager, GameplaySetupViewController gameplaySetupViewController, MultiplayerSettingsPanelController multiplayerSettingsPanelController)
        {
            _pluginConfig = pluginConfig;
            _screenCanvasManager = screenCanvasManager;
            _timeTweeningManager = timeTweeningManager;
            _gameplaySetupViewController = gameplaySetupViewController;
            _multiplayerSettingsPanelController = multiplayerSettingsPanelController;
        }

        [UIValue("on-screen-text-enabled")]
        private bool OnScreenTextEnabled
        {
            get => _pluginConfig.ScreenTextEnabled;
            set
            {
                _pluginConfig.ScreenTextEnabled = value;

                if (value)
                { 
                    _screenCanvasManager.ShowText(_pluginConfig.ScreenTextInTransitionAnimation, fade: _pluginConfig.ScreenTextInFade);   
                }
                else
                {
                    _screenCanvasManager.HideText(_pluginConfig.ScreenTextOutTransitionAnimation, fade: _pluginConfig.ScreenTextOutFade);
                }
            }
        }
        
        [UIValue("command-enabled")]
        private bool CommandEnabled
        {
            get => MultiCodeFields.DependencyInstalled && _pluginConfig.CommandEnabled;
            set
            {
                _pluginConfig.CommandEnabled = value;

                if (_multiCodeButtonUnderline != null)
                {
                    TweenMultiCodeButtonUnderlineColor(_multiCodeButtonUnderline, CommandEnabled ? Color.green : Color.red);
                }
            }
        }

        [UIValue("post-code-on-lobby-join")]
        private bool PostCodeOnLobbyJoin
        {
            get => MultiCodeFields.DependencyInstalled && _pluginConfig.PostCodeOnLobbyJoin;
            set => _pluginConfig.PostCodeOnLobbyJoin = value;
        }

        [UIValue("dependency-installed")] 
        private static bool DependencyInstalled => MultiCodeFields.DependencyInstalled;

        [UIAction("multi-code-button-clicked")]
        private void MultiCodeButtonClicked()
        {
            if (_multiCodeButton != null)
            {
                ShowModal(_multiCodeButton.transform);
            }
        }
        
        private void ShowModal(Component parentTransform)
        {
            if (!_parsed)
            {
                BSMLParser.instance.Parse(
                    Utilities.GetResourceContent(Assembly.GetExecutingAssembly(),
                        "MultiCode_inator.UI.Views.CommandToggleModalView.bsml"), parentTransform.gameObject, this);
                
                _parsed = true;
            }
            
            _parserParams.EmitEvent("close-modal");
            _parserParams.EmitEvent("open-modal");
        }

        private async void GameplaySetupViewControllerOndidActivateEvent(bool firstactivation, bool addedtohierarchy, bool screensystemenabling)
        {
            _gameplaySetupViewController.didActivateEvent -= GameplaySetupViewControllerOndidActivateEvent;

            await SiraUtil.Extras.Utilities.PauseChamp;
            _serverCodeButton = _multiplayerSettingsPanelController
                .GetField<ServerCodeView, MultiplayerSettingsPanelController>("_serverCodeView")
                .GetField<Button, ServerCodeView>("_button");
            _multiCodeButton = Object.Instantiate(_serverCodeButton, _serverCodeButton.transform.parent);
            _multiCodeButton.gameObject.name = "MultiCodeButton";
            _multiCodeButton.SetButtonText("MC");
            
            Object.Destroy(_multiCodeButton.transform.GetComponentInChildren<LayoutElement>());

            var contentSizeFitter = _multiCodeButton.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var transform = _multiCodeButton.transform;
            var position = transform.position;
            transform.position = new Vector3(position.x + 0.1f, position.y, position.z);
            transform.localScale = new Vector3(1f, 1f);
            
            _multiCodeButtonUnderline = _multiCodeButton.transform.Find("Underline").GetComponent<ImageView>();
            _multiCodeButtonUnderline.color = CommandEnabled ? Color.green : Color.red;
            
            _multiCodeButton.onClick.AddListener(MultiCodeButtonClicked);
        }

        private void TweenMultiCodeButtonUnderlineColor(ImageView underline, Color color)
        {
            _timeTweeningManager.KillAllTweens(underline);
            _timeTweeningManager.AddTween(
                new ColorTween(underline.color, color, value => underline.color = value, 0.4f, EaseType.OutQuad),
                underline);
        }

        public void Initialize() => _gameplaySetupViewController.didActivateEvent += GameplaySetupViewControllerOndidActivateEvent;

        public void Dispose() => _gameplaySetupViewController.didActivateEvent += GameplaySetupViewControllerOndidActivateEvent;
    }
}