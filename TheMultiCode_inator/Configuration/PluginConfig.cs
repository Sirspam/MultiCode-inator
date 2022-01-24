using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace TheMultiCode_inator.Configuration
{
    internal class PluginConfig
    {
        public virtual bool CommandEnabled { get; set; } = true;
    }
}