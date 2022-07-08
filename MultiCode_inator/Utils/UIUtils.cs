using HMUI;
using Tweening;
using UnityEngine;

namespace MultiCode_inator.Utils
{
	internal class UIUtils
	{
		private readonly TimeTweeningManager _uwuTweenyManager;

		public UIUtils(TimeTweeningManager timeTweeningManager)
		{
			_uwuTweenyManager = timeTweeningManager;
		}

		public void TweenMultiCodeButtonUnderlineColor(ImageView underline, Color color)
		{
			_uwuTweenyManager.KillAllTweens(underline);
			_uwuTweenyManager.AddTween(
				new ColorTween(underline.color, color, value => underline.color = value, 0.4f, EaseType.OutQuad),
				underline);
		}
	}
}