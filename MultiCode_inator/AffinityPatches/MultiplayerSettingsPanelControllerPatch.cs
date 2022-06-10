using MultiCode_inator.Utils;
using SiraUtil.Affinity;

namespace MultiCode_inator.AffinityPatches
{
    // Could've gotten the code via BGNet but I'm not certain if BeatTogether changes it in some way
    // This method is probably safer
    // [HarmonyPatch(typeof(MultiplayerSettingsPanelController), "SetLobbyCode", MethodType.Normal)]
    internal class MultiplayerSettingsPanelControllerPatch : IAffinity
    {
        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerSettingsPanelController), nameof(MultiplayerSettingsPanelController.SetLobbyCode))]
        private void Patch(string? code)
        {
            StaticFields.RoomCode = code;
        }
    }
}