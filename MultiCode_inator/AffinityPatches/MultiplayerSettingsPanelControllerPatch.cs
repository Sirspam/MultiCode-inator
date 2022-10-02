using MultiCode_inator.Managers;
using MultiCode_inator.Utils;
using SiraUtil.Affinity;

namespace MultiCode_inator.AffinityPatches
{
    internal class MultiplayerSettingsPanelControllerPatch : IAffinity
    {
        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerSettingsPanelController), nameof(MultiplayerSettingsPanelController.SetLobbyCode))]
        private void SetLobbyCodePatch(string code)
        {
            code = code.ToUpper();

            if (code == MultiCodeFields.RoomCode)
            {
                return;
            }
            
            MultiCodeFields.RoomCode = code;
        }
    }
}