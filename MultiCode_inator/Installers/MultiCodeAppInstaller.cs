using MultiCode_inator.AffinityPatches;
using MultiCode_inator.Configuration;
using MultiCode_inator.Utils;
using Zenject;

namespace MultiCode_inator.Installers
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
        }
    }
}