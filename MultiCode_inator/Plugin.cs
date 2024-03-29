﻿using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using MultiCode_inator.Configuration;
using MultiCode_inator.Installers;
using SiraUtil.Zenject;

namespace MultiCode_inator
{
    [Plugin(RuntimeOptions.DynamicInit)][NoEnableDisable]
    public class Plugin
    {
        [Init]
        public Plugin(Config conf, Logger logger, Zenjector zenjector)
        {
            zenjector.UseSiraSync();
            zenjector.UseLogger(logger);
            zenjector.UseMetadataBinder<Plugin>();
            
            zenjector.Install<MultiCodeAppInstaller>(Location.App, logger, conf.Generated<PluginConfig>());
            zenjector.Install<MultiCodeMenuInstaller>(Location.Menu);
        }
    }
}