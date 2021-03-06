using IPA.Loader;

namespace MultiCode_inator.Utils
{
    public static class StaticFields
    {
        private static bool? _catCoreInstalled;
        private static bool? _beatSaberPlusInstalled;
        
        public static string? RoomCode;
        public static bool CatCoreInstalled
        {
            get
            {
#if NO_DEPENDENCIES_DEBUG
	            return false;
#endif
#if BSP_DEBUG
                return false;   
#endif

                _catCoreInstalled ??= PluginManager.GetPluginFromId("CatCore") != null;
                return (bool) _catCoreInstalled;
            }
        }
        public static bool BeatSaberPlusInstalled
        {
            get
            {
#if NO_DEPENDENCIES_DEBUG
                return false;
#endif
#if CATCORE_DEBUG
                return false;          
#endif
                
                _beatSaberPlusInstalled ??= PluginManager.GetPluginFromId("BeatSaberPlusCORE") != null;
                return (bool) _beatSaberPlusInstalled;
            }
        }
        public static readonly bool DependencyInstalled = CatCoreInstalled || BeatSaberPlusInstalled;
        public const string NoDependenciesMessage = "Neither CatCore or BeatSaberPlus is installed. MultiCode-inator won't work without one of these mods installed!";
    }
}