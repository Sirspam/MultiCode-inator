using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities;
using MultiCode_inator.Configuration;
using MultiCode_inator.Managers;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace MultiCode_inator.UI.ViewControllers
{
	[HotReload(RelativePathToLayout = @"..\Views\MultiCodeSettingsView.bsml")]
	[ViewDefinition("MultiCode_inator.UI.Views.MultiCodeSettingsView.bsml")]
	internal class MultiCodeSettingsViewController : BSMLAutomaticViewController
	{
		private bool _modalParsed;
		private bool _previewTransitionButtonState;
		private ImageView _previewTransitionButtonUnderline = null!;
		private ScreenCanvasManager.TransitionType _currentModalTransitionType;
		
		[UIParams] 
		private readonly BSMLParserParams _parserParams = null!;
		
		private SiraLog _siraLog = null!;
		private PluginConfig _pluginConfig = null!;
		private ScreenCanvasManager _screenCanvasManager = null!;

		[Inject]
		public void Construct(SiraLog siraLog, PluginConfig pluginConfig, ScreenCanvasManager screenCanvasManager)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_screenCanvasManager = screenCanvasManager;
		}

		[UIValue("screen-text-enabled")]
		private bool ScreenTextEnabled
		{
			get => _pluginConfig.ScreenTextEnabled;
			set => _pluginConfig.ScreenTextEnabled = value;
		}

		[UIValue("screen-text-text")]
		private string ScreenTextText
		{
			get => _pluginConfig.ScreenText;
			set
			{
				_screenCanvasManager.SetText(value);
				_pluginConfig.ScreenText = value;
				NotifyPropertyChanged();
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
					_screenCanvasManager.ShowText(_pluginConfig.ScreenTextInTransitionAnimation, fade: _pluginConfig.ScreenTextInFade, loop: true);
				}
				else
				{
					_screenCanvasManager.HideText(_pluginConfig.ScreenTextOutTransitionAnimation, fade: _pluginConfig.ScreenTextOutFade, loop: true);
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

		[UIAction("change-text-clicked")]
		private void ChangeTextClicked()
		{
			ShowKeyboard();
		}

		[UIAction("keyboard-entered")]
		private void KeyboardEntered(string content)
		{
			ScreenTextText = content;
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

		private void OnEnable()
		{
			_screenCanvasManager.ShowText();
		}

		private void OnDisable()
		{
			_screenCanvasManager.HideText();
		}
	}
}