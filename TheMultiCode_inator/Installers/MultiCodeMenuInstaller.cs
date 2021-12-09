using TheMultiCode_inator.UI.ViewControllers;
using Zenject;

namespace TheMultiCode_inator.Installers
{
    internal class MultiCodeMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CommandToggleController>().AsSingle();
        }
    }
}