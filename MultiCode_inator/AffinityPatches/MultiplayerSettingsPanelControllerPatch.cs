using SiraUtil.Affinity;
using TheMultiCode_inator.Utils;

namespace TheMultiCode_inator.AffinityPatches
{
    // Could've gotten the code via BGNet but I'm not certain if BeatTogether changes it in some way
    // This method is probably safer
    // [HarmonyPatch(typeof(MultiplayerSettingsPanelController), "SetLobbyCode", MethodType.Normal)]
    internal class MultiplayerSettingsPanelControllerPatch : IAffinity
    {
        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerSettingsPanelController), nameof(MultiplayerSettingsPanelController.SetLobbyCode))]
        private void Postfix(string code)
        {
            CodeManager.RoomCode = code;
        }
    }
}