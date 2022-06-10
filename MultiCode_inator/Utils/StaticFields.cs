using IPA.Loader;

namespace MultiCode_inator.Utils
{
    public static class StaticFields
    {
        public static string? RoomCode { get; set; }
        public static readonly bool CatCoreInstalled = PluginManager.GetPluginFromId("CatCore") != null;
        public static readonly bool BeatSaberPlusInstalled = PluginManager.GetPluginFromId("BeatSaberPlusCORE") != null;
        public static readonly bool PluginEnabled = CatCoreInstalled && BeatSaberPlusInstalled;
        public const string NoDependenciesMessage = "Neither CatCore or BeatSaberPlus is installed. MultiCode-inator won't work without one of these mods installed!";
    }
}