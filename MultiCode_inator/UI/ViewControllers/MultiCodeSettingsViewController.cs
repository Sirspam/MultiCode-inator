using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Loader;
using IPA.Utilities;
using MultiCode_inator.Configuration;
using MultiCode_inator.Managers;
using MultiCode_inator.Utils;
using SiraUtil.Logging;
using SiraUtil.Web.SiraSync;
using SiraUtil.Zenject;
using TMPro;
using Tweening;
using UnityEngine;
using Zenject;

namespace MultiCode_inator.UI.ViewControllers
{
	[HotReload(RelativePathToLayout = @"..\Views\MultiCodeSettingsView.bsml")]
	[ViewDefinition("MultiCode_inator.UI.Views.MultiCodeSettingsView.bsml")]
	internal class MultiCodeSettingsViewController : BSMLAutomaticViewController
	{
		private bool _modalParsed;
		private bool _updateAvailable;
		private bool _previewTransitionButtonState;
		private ImageView _previewTransitionButtonUnderline = null!;
		private ScreenCanvasManager.TransitionType _currentModalTransitionType;
		
		[UIParams] 
		private readonly BSMLParserParams _parserParams = null!;
		
		[UIComponent("update-text")] 
		private readonly TextMeshProUGUI _updateText = null!;
		[UIComponent("version-text")] 
		private readonly CurvedTextMeshPro _versionText = null!;
		
		private SiraLog _siraLog = null!;
		private PluginConfig _pluginConfig = null!;
		private PluginMetadata _pluginMetadata = null!;
		private ISiraSyncService _siraSyncService = null!;
		private ScreenCanvasManager _screenCanvasManager = null!;
		private TimeTweeningManager _timeTweeningManager = null!;
		private GitHubPageModalController _gitHubPageModalController = null!;

		[Inject]
		public void Construct(SiraLog siraLog, PluginConfig pluginConfig, UBinder<Plugin, PluginMetadata> pluginMetadata, ISiraSyncService siraSyncServiceType, ScreenCanvasManager screenCanvasManager, TimeTweeningManager timeTweeningManager, GitHubPageModalController gitHubPageModalController)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_pluginMetadata = pluginMetadata.Value;
			_siraSyncService = siraSyncServiceType;
			_screenCanvasManager = screenCanvasManager;
			_timeTweeningManager = timeTweeningManager;
			_gitHubPageModalController = gitHubPageModalController;
		}
		
		[UIValue("update-available")]
		private bool UpdateAvailable
		{
			get => _updateAvailable;
			set
			{
				_updateAvailable = value;
				NotifyPropertyChanged();
			}
		}
		
		[UIValue("version-text-value")]
		private string VersionText => $"{_pluginMetadata.Name} v{_pluginMetadata.HVersion} by {_pluginMetadata.Author}";
		
		#region ScreenText

		[UIValue("screen-text-enabled")]
		private bool ScreenTextEnabled
		{
			get => _pluginConfig.ScreenTextEnabled;
			set => _pluginConfig.ScreenTextEnabled = value;
		}
		
		[UIValue("screen-text-string")]
		private string ScreenTextString
		{
			get => _pluginConfig.ScreenText;
			set
			{
				_screenCanvasManager.SetText(value);
				_pluginConfig.ScreenText = value;
				NotifyPropertyChanged(nameof(ScreenTextStringLimited));
				NotifyPropertyChanged();
			}
		}
		
		[UIValue("screen-text-string-limited")]
		private string ScreenTextStringLimited
		{
			get
			{
				var value = _pluginConfig.ScreenText;
				
				if (value.Length > 32)
				{
					value = value.Substring(0, 32) + "..";
				}

				return value;
			}
		}
		
		[UIValue("screen-text-font-size")]
		private int ScreenTextFontSize
		{
			get => _pluginConfig.ScreenTextFontSize;
			set
			{
				_screenCanvasManager.SetFontSize(_pluginConfig.ScreenTextFontSize);
				_pluginConfig.ScreenTextFontSize = value;
			}
		}
		
		[UIValue("screen-text-font-color")]
		private Color ScreenTextFontColor
		{
			get => _pluginConfig.ScreenTextFontColor;
			set
			{
				_screenCanvasManager.SetFontColor(value);
				_pluginConfig.ScreenTextFontColor = value;
			}
		}
		
		[UIValue("screen-text-italic-text")]
		private bool ScreenTextItalicText
		{
			get => _pluginConfig.ScreenTextItalicText;
			set
			{
				_screenCanvasManager.SetItalicFont(value);
				_pluginConfig.ScreenTextItalicText = value;
			}
		}
		
		[UIValue("screen-text-vertical-position")]
		private float ScreenTextVerticalPosition
		{
			get => _pluginConfig.ScreenTextPosition.x;
			set => _pluginConfig.ScreenTextPosition = _screenCanvasManager.SetTextNormalisedPosition(value, null);
		}
		
		[UIValue("screen-text-horizontal-position")]
		private float ScreenTextHorizontalPosition
		{
			get => _pluginConfig.ScreenTextPosition.y;
			set => _pluginConfig.ScreenTextPosition = _screenCanvasManager.SetTextNormalisedPosition(null, value);
		}
		
		[UIAction("change-text-clicked")]
		private void ChangeTextClicked()
		{
			ShowKeyboard();
		}

		[UIAction("keyboard-entered")]
		private void KeyboardEntered(string content)
		{
			ScreenTextString = content;
		}
		
		[UIAction("in-transition-clicked")]
		private void InTransitionClicked()
		{
			ShowModal(ScreenCanvasManager.TransitionType.In);
		}
		
		[UIAction("out-transition-clicked")]
		private void OutTransitionClicked()
		{
			ShowModal(ScreenCanvasManager.TransitionType.Out);
		}

		private void ShowKeyboard()
		{
			
			_parserParams.EmitEvent("open-keyboard");
		}

		#region TransitionModal

		[UIComponent("modal-view")] private readonly ModalView _modalView = null!;
		[UIComponent("animation-dropdown")] private readonly Transform _animationDropdown = null!;
		[UIComponent("preview-transition-button")] private readonly Transform _previewTransitionButton = null!;

		[UIValue("modal-title")]
		private string ModalTitle => $"{_currentModalTransitionType.ToString()} Transition";

		[UIValue("fade-value")]
		private bool FadeValue
		{
			get => _currentModalTransitionType == ScreenCanvasManager.TransitionType.In ? _pluginConfig.ScreenTextInFade : _pluginConfig.ScreenTextOutFade;
			set
			{
				ModalValueChanged();
				
				if (_currentModalTransitionType == ScreenCanvasManager.TransitionType.In)
				{
					_pluginConfig.ScreenTextInFade = value;
				}
				else
				{
					_pluginConfig.ScreenTextOutFade = value;
				}
			}
		}
		
		[UIValue("animation-types-options")]
		private List<object> AnimationTypesOptions => Enum.GetValues(typeof(ScreenCanvasManager.TransitionAnimation)).Cast<object>().ToList();

		[UIValue("animation-types-value")]
		private ScreenCanvasManager.TransitionAnimation AnimationTypesValue
		{
			get => _currentModalTransitionType == ScreenCanvasManager.TransitionType.In ? _pluginConfig.ScreenTextInTransitionAnimation : _pluginConfig.ScreenTextOutTransitionAnimation;
			set
			{
				ModalValueChanged();
				
				if (_currentModalTransitionType == ScreenCanvasManager.TransitionType.In)
				{
					_pluginConfig.ScreenTextInTransitionAnimation = value;
				}
				else
				{
					_pluginConfig.ScreenTextOutTransitionAnimation = value;
				}
			}
		}

		[UIAction("preview-transition-clicked")]
		private void PreviewTransitionClicked()
		{
			if (!_previewTransitionButtonState)
			{
				_previewTransitionButtonState = true;
				_previewTransitionButtonUnderline.color = Color.green;

				if (_currentModalTransitionType == ScreenCanvasManager.TransitionType.In)
				{
					_screenCanvasManager.ShowText(_pluginConfig.ScreenTextInTransitionAnimation, _pluginConfig.ScreenTextInFade, loop: true);
				}
				else
				{
					_screenCanvasManager.HideText(_pluginConfig.ScreenTextOutTransitionAnimation, _pluginConfig.ScreenTextOutFade, loop: true);
				}
				
			}
			else
			{
				_previewTransitionButtonState = false;
				_previewTransitionButtonUnderline.color = Color.grey;
				
				_screenCanvasManager.KillLoop();
			}
		}
		
		private void ShowModal(ScreenCanvasManager.TransitionType transitionType)
		{
			if (!_modalParsed)
			{
				_modalView.name = "MultiCodeTransitionModal";
				_modalView.SetField("_animateParentCanvas", true);
				_animationDropdown.Find("DropdownTableView").GetComponent<ModalView>().SetField("_animateParentCanvas", false);
				_previewTransitionButtonUnderline = _previewTransitionButton.Find("Underline").GetComponent<ImageView>();
				_modalParsed = true;
			}
			
			_currentModalTransitionType = transitionType;
			NotifyPropertyChanged(nameof(ModalTitle));
			NotifyPropertyChanged(nameof(AnimationTypesOptions));
			NotifyPropertyChanged(nameof(AnimationTypesValue));
			_modalView.blockerClickedEvent += ModalViewOnBlockerClickedEvent;
			_parserParams.EmitEvent("open-modal");
		}

		private void ModalValueChanged()
		{
			if (_previewTransitionButtonState)
			{
				PreviewTransitionClicked();
			}
		}

		private void ModalViewOnBlockerClickedEvent()
		{
			_modalView.blockerClickedEvent -= ModalViewOnBlockerClickedEvent;
			if (_previewTransitionButtonState)
			{
				PreviewTransitionClicked();
			}
		}

		#endregion

		#endregion

		#region StreamCommand

		[UIComponent("current-broadcaster")] private readonly TextMeshProUGUI _currentBroadcaster = null!;
		
		[UIValue("command-enabled")]
		private bool CommandEnabled
		{
			get => MultiCodeFields.DependencyInstalled && _pluginConfig.CommandEnabled;
			set => _pluginConfig.CommandEnabled = value;
		}

		[UIValue("post-code-on-lobby-join")]
		private bool PostCodeOnLobbyJoin
		{
			get => MultiCodeFields.DependencyInstalled && _pluginConfig.PostCodeOnLobbyJoin;
			set => _pluginConfig.PostCodeOnLobbyJoin = value;
		}

		[UIValue("current-broadcaster-text")]
		private string CurrentBroadcasterText
		{
			get
			{
				_currentBroadcaster.color = Color.green;
				
				if (MultiCodeFields.CatCoreInstalled)
				{
					return "CatCore";
				}

				if (MultiCodeFields.BeatSaberPlusInstalled)
				{
					return "BeatSaberPlus";
				}

				_currentBroadcaster.color = Color.red;
				return "None";
			}
		}

		[UIValue("dependency-installed")] 
		private static bool DependencyInstalled => MultiCodeFields.DependencyInstalled;

		[UIValue("missing-dependency-text")] 
		private string MissingDependencyText => MultiCodeFields.NoDependenciesMessage;

		#endregion

		
		[UIAction("#post-parse")]
		private async void PostParse()
		{
			if (DependencyInstalled)
			{
				_currentBroadcaster.color = Color.green;
				
				if (MultiCodeFields.CatCoreInstalled)
				{
					_currentBroadcaster.text = "CatCore";
				}

				if (MultiCodeFields.BeatSaberPlusInstalled)
				{
					_currentBroadcaster.text = "BeatSaberPlus";
				}	
			}

			else
			{
				_currentBroadcaster.color = Color.red;
				_currentBroadcaster.text = "None";	
			}

			var gitVersion = await _siraSyncService.LatestVersion();
			if (gitVersion != null && gitVersion > _pluginMetadata.HVersion)
			{
				var message = $"MultiCode-inator v{gitVersion} is available on GitHub!";
				_siraLog.Info(message);
				_updateText.text = message;
				_updateText.alpha = 0;
				UpdateAvailable = true;
				_timeTweeningManager.AddTween(new FloatTween(0f, 1f, val => _updateText.alpha = val, 0.4f, EaseType.InCubic), this);
			}
		}
		
		[UIAction("version-text-clicked")]
		private void VersionTextClicked()
		{
			if (_pluginMetadata.PluginHomeLink == null)
			{
				return;
			}
			
			_gitHubPageModalController.ShowModal(_versionText.transform, _pluginMetadata.Name,
				_pluginMetadata.PluginHomeLink!.ToString());
		}

		private void OnEnable()
		{
			_screenCanvasManager.ShowText(_pluginConfig.ScreenTextInTransitionAnimation, _pluginConfig.ScreenTextInFade);
		}

		private void OnDisable()
		{
			_screenCanvasManager.HideText(_pluginConfig.ScreenTextOutTransitionAnimation, _pluginConfig.ScreenTextOutFade);
		}
	}
}