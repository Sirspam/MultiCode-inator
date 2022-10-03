using System;
using System.Threading;
using System.Threading.Tasks;
using MultiCode_inator.Configuration;
using MultiCode_inator.Utils;
using SiraUtil.Logging;
using TMPro;
using Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MultiCode_inator.Managers
{
	internal class ScreenCanvasManager : IInitializable, IDisposable
	{
		public enum TransitionAnimation
		{
			None,
			SlideUp,
			SlideDown,
			SlideLeft,
			SlideRight
		}

		public enum TransitionType
		{
			In,
			Out
		}

		private int? _halvedWidthResolution;
		private int? _halvedHeightResolution;
		private const int PositionOffset = 65;
		private const float DefaultDuration = 0.7f;
		
		private TextMeshProUGUI? _text;
		private GameObject? _canvasGameObject;
		private RectTransform? _textRectTransform;
		private CancellationTokenSource? _cancellationTokenSource;

		private readonly SiraLog _siraLog;
		private readonly PluginConfig _pluginConfig;
		private readonly TimeTweeningManager _timeTweeningManager;

		public ScreenCanvasManager(SiraLog siraLog, PluginConfig pluginConfig, TimeTweeningManager timeTweeningManager)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_timeTweeningManager = timeTweeningManager;
		}

		private void CreateText()
		{
			if (_canvasGameObject != null)
			{
				_canvasGameObject.SetActive(true);
				_text!.alpha = 1f;
				
				return;
			}
			
			_canvasGameObject = new GameObject
			{
				name = "MultiCodeTextCanvas"
			};

			var canvas = _canvasGameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 1;
			
			var textGameObject = new GameObject
			{
				transform =
				{
					parent = _canvasGameObject.transform
				},
				name = "Text"
			};

			_text = textGameObject.AddComponent<TextMeshProUGUI>();
			var contentSizeFitter = textGameObject.AddComponent<ContentSizeFitter>();
			contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			// On preferred size there's vertical padding and on min size the text can go OOB vertically
			// No clue how to fix, just going to use unconstrained and let the user cope
			contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			SetText(_pluginConfig.ScreenText);
			_text.alignment = TextAlignmentOptions.Center;
			_text.enableWordWrapping = false;
			_text.lineSpacing = 0;
			_text.fontSize = _pluginConfig.ScreenTextFontSize;
			_text.color = _pluginConfig.ScreenTextFontColor;
			SetItalicFont(_pluginConfig.ScreenTextItalicText);

			_textRectTransform = _text.GetComponent<RectTransform>();
			SetTextNormalisedPosition(_pluginConfig.ScreenTextPosition);
		}

		public void ShowText(TransitionAnimation transitionAnimation = TransitionAnimation.None, float duration = DefaultDuration, bool fade = false, bool loop = false)
		{
			CreateText();
			
			Vector2Tween? vector2Tween = null;
			var normalisedConfigPosition = NormalisedPositionToVector2(_pluginConfig.ScreenTextPosition);
			
			switch (transitionAnimation)
			{
				case TransitionAnimation.SlideUp:
				{
					vector2Tween =
						new Vector2Tween(new Vector2(normalisedConfigPosition.x, normalisedConfigPosition.y - PositionOffset), normalisedConfigPosition,
							val => _textRectTransform!.localPosition = val, duration, EaseType.OutQuad);
					break;
				}
				case TransitionAnimation.SlideDown:
				{
					vector2Tween =
						new Vector2Tween(new Vector2(normalisedConfigPosition.x, normalisedConfigPosition.y + PositionOffset), normalisedConfigPosition,
							val => _textRectTransform!.localPosition = val, duration, EaseType.OutQuad);
					break;
				}
				case TransitionAnimation.SlideLeft:
				{
					vector2Tween =
						new Vector2Tween(new Vector2(normalisedConfigPosition.x + PositionOffset, normalisedConfigPosition.y), normalisedConfigPosition,
							val => _textRectTransform!.localPosition = val, duration, EaseType.OutQuad);
					break;
				}
				case TransitionAnimation.SlideRight:
				{
					vector2Tween =
						new Vector2Tween(new Vector2(normalisedConfigPosition.x - PositionOffset, normalisedConfigPosition.y), normalisedConfigPosition,
							val => _textRectTransform!.localPosition = val, duration, EaseType.OutQuad);
					break;
				}
				default: case TransitionAnimation.None:
				{
					SetTextNormalisedPosition(_pluginConfig.ScreenTextPosition);
					break;
				}
			}
			
			if (vector2Tween != null)
			{
				if (loop)
				{
					vector2Tween.onCompleted = () => Task.Run(() => CancellableDelayAsync(() => ShowText(transitionAnimation, duration, fade, loop), (int) Math.Round(duration * 1000 + 500)));
				}

				vector2Tween.onKilled = () => SetTextNormalisedPosition(_pluginConfig.ScreenTextPosition);
				_timeTweeningManager.AddTween(vector2Tween, this);

				if (fade)
				{
					FadeText(0f, 1f, duration);
				}
			}
		}

		public void HideText(TransitionAnimation transitionAnimation = TransitionAnimation.None, float duration = DefaultDuration, bool fade = false, bool loop = false)
		{
			CreateText();
			
			Vector2Tween? vector2Tween = null;
			var normalisedConfigPosition = NormalisedPositionToVector2(_pluginConfig.ScreenTextPosition);
			
			switch (transitionAnimation)
			{
				case TransitionAnimation.SlideUp:
				{
					vector2Tween =
						new Vector2Tween(normalisedConfigPosition, new Vector2(normalisedConfigPosition.x, normalisedConfigPosition.y + PositionOffset),
							val => _textRectTransform!.localPosition = val, duration, EaseType.InQuad);
					break;
				}
				case TransitionAnimation.SlideDown:
				{
					vector2Tween =
						new Vector2Tween(normalisedConfigPosition, new Vector2(normalisedConfigPosition.x, normalisedConfigPosition.y - PositionOffset),
							val => _textRectTransform!.localPosition = val, duration, EaseType.InQuad);
					break;
				}
				case TransitionAnimation.SlideLeft:
				{
					vector2Tween =
						new Vector2Tween(normalisedConfigPosition, new Vector2(normalisedConfigPosition.x - PositionOffset, normalisedConfigPosition.y),
							val => _textRectTransform!.localPosition = val, duration, EaseType.InQuad);
					break;
				}
				case TransitionAnimation.SlideRight:
				{
					vector2Tween =
						new Vector2Tween(normalisedConfigPosition, new Vector2(normalisedConfigPosition.x + PositionOffset, normalisedConfigPosition.y),
							val => _textRectTransform!.localPosition = val, duration, EaseType.InQuad);
					break;
				}
				default: case TransitionAnimation.None:
				{
					_canvasGameObject!.SetActive(false);
					break;
				}
			}
			
			if (vector2Tween != null)
			{
				if (loop)
				{
					vector2Tween.onCompleted = () => Task.Run(() => CancellableDelayAsync(() => HideText(transitionAnimation, duration, fade, loop),  (int) Math.Round(duration * 1000 + 500)));
				}

				vector2Tween.onKilled = () => SetTextNormalisedPosition(_pluginConfig.ScreenTextPosition);
				_timeTweeningManager.AddTween(vector2Tween, this);

				if (fade)
				{
					FadeText(1f, 0f, duration);
				}
			}
		}

		private async Task CancellableDelayAsync(Action onCompleted, int duration)
		{
			_cancellationTokenSource = new CancellationTokenSource();
						
			try
			{
				await Task.Delay(duration, _cancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			
			onCompleted.Invoke();
		}

		private void FadeText(float fromValue, float toValue, float duration)
		{
			CreateText();
			
			var fadeTween = new FloatTween(fromValue, toValue, val => _text!.alpha = val, duration, EaseType.OutSine)
			{
				onKilled = () => _text!.alpha = 1f
			};
			
			_timeTweeningManager.AddTween(fadeTween, this);
		}

		public void KillLoop()
		{
			_timeTweeningManager.KillAllTweens(this);
			_cancellationTokenSource?.Cancel(false);

			SetTextNormalisedPosition(_pluginConfig.ScreenTextPosition);
		}

		public Vector2 SetTextNormalisedPosition(float? newX, float? newY)
		{
			CreateText();

			Vector2 vector;
			if (newX != null && newY != null)
			{
				vector = new Vector2((float) newX, (float) newY);
			}

			else if (newX != null && newY == null)
			{
				vector = new Vector2((float) newX, _pluginConfig.ScreenTextPosition.y);
			}

			else if (newX == null && newY != null)
			{
				vector = new Vector2(_pluginConfig.ScreenTextPosition.x, (float) newY);
			}
			else // both parameters null
			{
				return _pluginConfig.ScreenTextPosition;
			}
			
			SetTextNormalisedPosition(vector);
			return vector;
		}

		private void SetTextNormalisedPosition(Vector2 newPosition)
		{
			CreateText();

			if (_textRectTransform!.localPosition.Equals(newPosition))
			{
				return;
			}

			_textRectTransform.localPosition = NormalisedPositionToVector2(newPosition);
			_textRectTransform.pivot = newPosition;
		}

		private Vector2 NormalisedPositionToVector2(Vector2 position)
		{
			_halvedWidthResolution ??= Screen.width / 2;
			_halvedHeightResolution ??= Screen.height / 2;
			return new Vector2(
				Mathf.Lerp((int) _halvedWidthResolution * -1, (int) _halvedWidthResolution, position.x), 
				Mathf.Lerp((int) _halvedHeightResolution * -1, (int) _halvedHeightResolution, position.y));
		}

		public void SetText(string text)
		{
			CreateText();
			
			var code = MultiCodeFields.RoomCode ?? "E970"; // My beloved ❤
			_text!.text = text.Replace("{code}", code);
		}
		
		public void SetFontSize(int size)
		{
			CreateText();

			_text!.fontSize = size;
			SetTextNormalisedPosition(_pluginConfig.ScreenTextPosition);
		}

		public void SetFontColor(Color color)
		{
			CreateText();

			_text!.color = color;
		}

		public void SetItalicFont(bool active)
		{
			CreateText();
			
			_text!.fontStyle = active ? FontStyles.Italic : FontStyles.Normal;
		}

		private void MultiCodeFieldsOnLobbyCodeUpdated(string? code)
		{
			if (!_pluginConfig.ScreenTextEnabled)
			{
				return;
			}
			
			if (code != null)
			{
				SetText(_pluginConfig.ScreenText);
				ShowText(_pluginConfig.ScreenTextInTransitionAnimation, fade: _pluginConfig.ScreenTextInFade);
			}
			else
			{
				HideText(_pluginConfig.ScreenTextOutTransitionAnimation, fade: _pluginConfig.ScreenTextOutFade);
			}
		}

		public void Initialize()
		{
			MultiCodeFields.LobbyCodeUpdated += MultiCodeFieldsOnLobbyCodeUpdated;
		}

		public void Dispose()
		{
			MultiCodeFields.LobbyCodeUpdated -= MultiCodeFieldsOnLobbyCodeUpdated;
		}
	}
}