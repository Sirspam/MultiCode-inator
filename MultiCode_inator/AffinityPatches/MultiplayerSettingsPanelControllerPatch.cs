using MultiCode_inator.Managers;
using MultiCode_inator.Utils;
using SiraUtil.Affinity;

namespace MultiCode_inator.AffinityPatches
{
    internal class MultiplayerSettingsPanelControllerPatch : IAffinity
    {
        private readonly BroadcastManager _broadcastManager;

        public MultiplayerSettingsPanelControllerPatch(BroadcastManager broadcastManager)
        {
            _broadcastManager = broadcastManager;
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerSettingsPanelController), nameof(MultiplayerSettingsPanelController.SetLobbyCode))]
        private void SetLobbyCodePatch(string code)
        {
            code = code.ToUpper();

            if (code == StaticFields.RoomCode)
            {
                return;
            }
            
            StaticFields.RoomCode = code;
            _broadcastManager.LobbyCodeUpdated(code);
        }
    }
}