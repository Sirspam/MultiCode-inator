using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace TheMultiCode_inator.Configuration
{
    internal class PluginConfig
    {
        public virtual bool CommandEnabled { get; set; } = true;
    }
}