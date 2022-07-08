﻿using System;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using MultiCode_inator.Configuration;
using MultiCode_inator.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace MultiCode_inator.UI.ViewControllers
{
    internal class CommandToggleViewController : IInitializable, IDisposable
    {
        private bool _parsed;
        
        private Button? _multiCodeButton;
        private Button? _serverCodeButton;
        private ImageView? _multiCodeButtonUnderline;

        [UIParams] 
        private readonly BSMLParserParams _parserParams = null!;

        private readonly UIUtils _uiUtils;
        private readonly PluginConfig _pluginConfig;
        private readonly GameplaySetupViewController _gameplaySetupViewController;
        private readonly MultiplayerSettingsPanelController _multiplayerSettingsPanelController;

        public CommandToggleViewController(UIUtils uiUtils, PluginConfig pluginConfig, GameplaySetupViewController gameplaySetupViewController, MultiplayerSettingsPanelController multiplayerSettingsPanelController)
        {
            _uiUtils = uiUtils;
            _pluginConfig = pluginConfig;
            _gameplaySetupViewController = gameplaySetupViewController;
            _multiplayerSettingsPanelController = multiplayerSettingsPanelController;
        }

        [UIValue("size-delta-y")] 
        private int ModalSizeDeltaY => DependencyInstalled ? 34 : 54;
        
        [UIValue("modal-pref-height")] 
        private int ModalPrefHeight => ModalSizeDeltaY - 5;

        [UIValue("command-enabled")]
        private bool CommandEnabled
        {
            get => StaticFields.DependencyInstalled && _pluginConfig.CommandEnabled;
            set
            {
                _pluginConfig.CommandEnabled = value;

                if (_multiCodeButtonUnderline != null)
                {
                    _uiUtils.TweenMultiCodeButtonUnderlineColor(_multiCodeButtonUnderline, CommandEnabled ? Color.green : Color.red);
                }
            }
        }

        [UIValue("post-code-on-lobby-join")]
        private bool PostCodeOnLobbyJoin
        {
            get => StaticFields.DependencyInstalled && _pluginConfig.PostCodeOnLobbyJoin;
            set => _pluginConfig.PostCodeOnLobbyJoin = value;
        }

        [UIValue("dependency-installed")] 
        private static bool DependencyInstalled => StaticFields.DependencyInstalled;

        [UIValue("missing-dependency-text")] 
        private string MissingDependencyText => StaticFields.NoDependenciesMessage;

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
        
        public void Initialize()
        {
            _gameplaySetupViewController.didActivateEvent += GameplaySetupViewControllerOndidActivateEvent;
        }

        private void GameplaySetupViewControllerOndidActivateEvent(bool firstactivation, bool addedtohierarchy, bool screensystemenabling)
        {
            _gameplaySetupViewController.didActivateEvent -= GameplaySetupViewControllerOndidActivateEvent;
            
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

        public void Dispose()
        {
            _gameplaySetupViewController.didActivateEvent += GameplaySetupViewControllerOndidActivateEvent;
        }
    }
}