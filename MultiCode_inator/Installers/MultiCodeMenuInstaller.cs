using MultiCode_inator.UI.ViewControllers;
using Zenject;

namespace MultiCode_inator.Installers
{
    internal class MultiCodeMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CommandToggleViewController>().AsSingle();
        }
    }
}