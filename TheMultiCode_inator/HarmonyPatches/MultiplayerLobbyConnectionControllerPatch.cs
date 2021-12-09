using HarmonyLib;
using TheMultiCode_inator.Utils;

namespace TheMultiCode_inator.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerLobbyConnectionController), "LeaveLobby", MethodType.Normal)]
    internal class LeaveLobbyPatch
    {
        internal static void Postfix(MultiplayerLocalPlayerDisconnectHelper __instance)
        {
            CodeManager.RoomCode = null;
        }
    }
}