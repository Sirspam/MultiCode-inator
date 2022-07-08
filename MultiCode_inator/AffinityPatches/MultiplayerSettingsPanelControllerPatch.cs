using System;
using MultiCode_inator.Utils;
using SiraUtil.Affinity;

namespace MultiCode_inator.AffinityPatches
{
    internal class MultiplayerSettingsPanelControllerPatch : IAffinity
    {
        public event Action<string> CodeSetEvent = null!; 

        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerSettingsPanelController), nameof(MultiplayerSettingsPanelController.SetLobbyCode))]
        private void SetLobbyCodePatch(string code)
        {
            StaticFields.RoomCode = code;
            CodeSetEvent.Invoke(code);
        }
    }
}