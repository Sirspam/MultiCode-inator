using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace MultiCode_inator.Configuration
{
    internal class PluginConfig
    {
        public virtual bool CommandEnabled { get; set; } = true;
        public virtual bool PostCodeOnLobbyJoin { get; set; } = true;
    }
}