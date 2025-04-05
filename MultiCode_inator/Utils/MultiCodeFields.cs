using System;
using IPA.Loader;
using Version = Hive.Versioning.Version;

namespace MultiCode_inator.Utils
{
    public static class MultiCodeFields
    {
	    public static event Action<string?>? LobbyCodeUpdated; 

	    private static bool? _catCoreInstalled;
        private static bool? _beatSaberPlusInstalled;
        
        private static string? _roomCode;

        public static string? RoomCode
        {
	        get => _roomCode;
	        set
	        {
		        _roomCode = value;
		        LobbyCodeUpdated?.Invoke(value);
	        }
        }

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
	            _beatSaberPlusInstalled ??= PluginManager.GetPluginFromId("ChatPlexSDK_BS") != null;
	            return (bool) _beatSaberPlusInstalled;
            }
        }

        public static readonly bool DependencyInstalled = CatCoreInstalled || BeatSaberPlusInstalled;
        public const string NoDependenciesMessage = "None of the optional broadcasters are installed. MultiCode-inator's chat features won't work without one installed! Refer to MultiCode-inator's README for a list of supported broadcasters";
    }
}