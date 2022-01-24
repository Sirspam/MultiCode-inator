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
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.github.Sirspam.TheMultiCode_inator";

        internal static readonly HarmonyLib.Harmony Harmony = new HarmonyLib.Harmony(HarmonyId);

        [Init]
        public Plugin(Config conf, Logger logger, Zenjector zenjector)
        {
            zenjector.UseLogger(logger);
            
            var config = conf.Generated<PluginConfig>();
            zenjector.Install<MultiCodeAppInstaller>(Location.App, config);
            zenjector.Install<MultiCodeMenuInstaller>(Location.Menu);
        }
        
        #region Harmony

        /// <summary>
        /// Attempts to apply all the Harmony patches in this assembly.
        /// </summary>
        internal static void ApplyHarmonyPatches()
        {
            try
            {
                Plugin.Log?.Debug("Applying Harmony patches.");
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("Error applying Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }

        /// <summary>
        /// Attempts to remove all the Harmony patches that used our HarmonyId.
        /// </summary>
        internal static void RemoveHarmonyPatches()
        {
            try
            {
                // Removes all patches with this HarmonyId
                Harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("Error removing Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }

        #endregion Harmony
    }
}