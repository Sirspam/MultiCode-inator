using SiraUtil.Affinity;
using MultiCode_inator.Utils;

namespace MultiCode_inator.AffinityPatches
{
    internal class MultiplayerLobbyConnectionControllerPatch : IAffinity
    {
        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerLobbyConnectionController), nameof(MultiplayerLobbyConnectionController.LeaveLobby))]
        private void Postfix()
        {
            CodeManager.RoomCode = null;
        }
    }
}