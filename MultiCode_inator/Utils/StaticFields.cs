using IPA.Loader;

namespace MultiCode_inator.Utils
{
    public static class StaticFields
    {
        public static string? RoomCode;
        public static bool CatCoreInstalled { get; } = PluginManager.GetPluginFromId("CatCore") != null;
        public static bool BeatSaberPlusInstalled { get; } = PluginManager.GetPluginFromId("BeatSaberPlusCORE") != null;
        public static readonly bool DependencyInstalled = CatCoreInstalled || BeatSaberPlusInstalled;
        public const string NoDependenciesMessage = "Neither CatCore or BeatSaberPlus is installed. MultiCode-inator won't work without one of these mods installed!";
    }
}