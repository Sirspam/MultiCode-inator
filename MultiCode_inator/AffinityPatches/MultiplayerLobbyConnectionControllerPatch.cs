using MultiCode_inator.Utils;
using SiraUtil.Affinity;

namespace MultiCode_inator.AffinityPatches
{
    internal class MultiplayerLobbyConnectionControllerPatch : IAffinity
    {
        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerLobbyConnectionController), nameof(MultiplayerLobbyConnectionController.LeaveLobby))]
        private void Patch()
        {
            StaticFields.RoomCode = null;
        }
    }
}