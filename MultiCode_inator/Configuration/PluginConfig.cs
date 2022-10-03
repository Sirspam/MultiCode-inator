using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using MultiCode_inator.Managers;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace MultiCode_inator.Configuration
{
    internal class PluginConfig
    {
        public virtual bool CommandEnabled { get; set; } = true;
        public virtual bool PostCodeOnLobbyJoin { get; set; } = true;
        public virtual bool ScreenTextEnabled { get; set; } = true;
        public virtual string ScreenText { get; set; } = "Lobby Code: {code}";
        public virtual int ScreenTextFontSize { get; set; } = 60;
        public virtual Color ScreenTextFontColor { get; set; } = Color.white;
        public virtual bool ScreenTextItalicText { get; set; } = true;
        public virtual Vector2 ScreenTextPosition { get; set; } = new Vector2(0.5f, 0f);
        [UseConverter(typeof(EnumConverter<ScreenCanvasManager.TransitionAnimation>))]
        public virtual ScreenCanvasManager.TransitionAnimation ScreenTextInTransitionAnimation { get; set; } = ScreenCanvasManager.TransitionAnimation.SlideDown;

        public virtual bool ScreenTextInFade { get; set; } = true;
        [UseConverter(typeof(EnumConverter<ScreenCanvasManager.TransitionAnimation>))]
        public virtual ScreenCanvasManager.TransitionAnimation ScreenTextOutTransitionAnimation { get; set; } = ScreenCanvasManager.TransitionAnimation.SlideUp;
        public virtual bool ScreenTextOutFade { get; set; } = true;
    }
}