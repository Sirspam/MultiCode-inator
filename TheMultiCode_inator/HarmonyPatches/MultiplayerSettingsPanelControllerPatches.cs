using HarmonyLib;
using TheMultiCode_inator.Utils;

namespace TheMultiCode_inator.HarmonyPatches
{
    // Could've gotten the code via BGNet but I'm not certain if BeatTogether changes it in some way
    // This method is probably safer
    [HarmonyPatch(typeof(MultiplayerSettingsPanelController), "SetLobbyCode", MethodType.Normal)]
    internal class SetLobbyCodePatch
    {
        internal static void Postfix(MultiplayerSettingsPanelController __instance, string code)
        {
            CodeManager.RoomCode = code;
        }
    }
}