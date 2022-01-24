using System;
using SiraUtil.Affinity;
using TheMultiCode_inator.Utils;

namespace TheMultiCode_inator.AffinityPatches
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