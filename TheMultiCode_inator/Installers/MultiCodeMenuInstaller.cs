using TheMultiCode_inator.UI.ViewControllers;
using TheMultiCode_inator.Utils;
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