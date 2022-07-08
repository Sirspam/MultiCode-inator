using IPA.Logging;
using MultiCode_inator.AffinityPatches;
using MultiCode_inator.Configuration;
using MultiCode_inator.Utils;
using Zenject;

namespace MultiCode_inator.Installers
{
    internal class MultiCodeAppInstaller : Installer
    {
        private readonly Logger _logger;
        private readonly PluginConfig _pluginConfig;

        public MultiCodeAppInstaller(Logger logger, PluginConfig pluginConfig)
        {
            _logger = logger;
            _pluginConfig = pluginConfig;
        }
        
        public override void InstallBindings()
        {
            Container.BindInstance(_pluginConfig).AsSingle();
            
            if (StaticFields.DependencyInstalled)
            {
                Container.BindInterfacesTo<MultiplayerLobbyConnectionControllerPatch>().AsSingle();
                Container.BindInterfacesAndSelfTo<MultiplayerSettingsPanelControllerPatch>().AsSingle();

                if (StaticFields.CatCoreInstalled)
                {
                    Container.BindInterfacesTo<CatCoreBroadcaster>().AsSingle();
                }
                else if (StaticFields.BeatSaberPlusInstalled)
                {
                    Container.BindInterfacesTo<BspBroadcaster>().AsSingle();
                }
            }
            else
            {
                _logger.Warn(StaticFields.NoDependenciesMessage);
            }
        }
    }
}