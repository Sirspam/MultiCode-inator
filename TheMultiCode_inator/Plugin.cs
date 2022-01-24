using IPA;
using IPA.Config;
using SiraUtil.Zenject;
using System;
using System.Reflection;
using IPA.Config.Stores;
using TheMultiCode_inator.Installers;
using IPA.Logging;
using TheMultiCode_inator.Configuration;

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