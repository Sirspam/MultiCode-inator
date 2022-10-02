using IPA.Logging;
using MultiCode_inator.AffinityPatches;
using MultiCode_inator.Broadcasters;
using MultiCode_inator.Configuration;
using MultiCode_inator.Managers;
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
            
            Container.BindInterfacesTo<MultiplayerLobbyConnectionControllerPatch>().AsSingle();
            Container.BindInterfacesAndSelfTo<MultiplayerSettingsPanelControllerPatch>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScreenCanvasManager>().AsSingle();
            
            if (MultiCodeFields.DependencyInstalled)
            {
                if (MultiCodeFields.CatCoreInstalled)
                {
                    _logger.Info($"Using {nameof(CatCoreBroadcaster)}");
                    Container.BindInterfacesTo<CatCoreBroadcaster>().AsSingle();
                }
                else if (MultiCodeFields.BeatSaberPlusInstalled)
                {
                    _logger.Info($"Using {nameof(BspBroadcaster)}");
                    Container.BindInterfacesTo<BspBroadcaster>().AsSingle();
                }

                Container.BindInterfacesAndSelfTo<BroadcastManager>().AsSingle();
            }
            else
            {
                _logger.Warn(MultiCodeFields.NoDependenciesMessage);
            }
        }
    }
}