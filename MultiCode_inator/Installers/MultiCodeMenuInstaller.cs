using MultiCode_inator.UI.ViewControllers;
using MultiCode_inator.Utils;
using Zenject;

namespace MultiCode_inator.Installers
{
    internal class MultiCodeMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<UIUtils>().AsSingle();
            Container.BindInterfacesTo<CommandToggleViewController>().AsSingle();
        }
    }
}