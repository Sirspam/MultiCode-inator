using MultiCode_inator.Utils;
using SiraUtil.Affinity;

namespace MultiCode_inator.AffinityPatches
{
    // Probably could've gotten the code via BGNet but I'm not certain if BeatTogether changes it in some way
    // This method is probably safer
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