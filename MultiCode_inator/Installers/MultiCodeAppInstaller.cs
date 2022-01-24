using TheMultiCode_inator.AffinityPatches;
using TheMultiCode_inator.Configuration;
using TheMultiCode_inator.Utils;
using Zenject;

namespace TheMultiCode_inator.Installers
{
    internal class MultiCodeAppInstaller : Installer
    {
        private readonly PluginConfig _pluginConfig;

        public MultiCodeAppInstaller(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }
        
        public override void InstallBindings()
        {
            Container.BindInstance(_pluginConfig).AsSingle();
            Container.BindInterfacesTo<CodeBroadcaster>().AsSingle();
            Container.BindInterfacesTo<MultiplayerLobbyConnectionControllerPatch>().AsSingle();
            Container.BindInterfacesTo<MultiplayerSettingsPanelControllerPatch>().AsSingle();
        }
    }
}