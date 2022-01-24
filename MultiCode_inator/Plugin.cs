using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using SiraUtil.Zenject;
using TheMultiCode_inator.Configuration;
using TheMultiCode_inator.Installers;

namespace TheMultiCode_inator
{
    [Plugin(RuntimeOptions.DynamicInit)][NoEnableDisable]
    public class Plugin
    {
        [Init]
        public Plugin(Config conf, Logger logger, Zenjector zenjector)
        {
            zenjector.UseLogger(logger);
            
            var config = conf.Generated<PluginConfig>();
            zenjector.Install<MultiCodeAppInstaller>(Location.App, config);
            zenjector.Install<MultiCodeMenuInstaller>(Location.Menu);
        }
    }
}