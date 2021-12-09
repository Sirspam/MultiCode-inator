using TheMultiCode_inator.UI.ViewControllers;
using TheMultiCode_inator.Utils;
using Zenject;

namespace TheMultiCode_inator.Installers
{
    internal class MultiCodeAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CodeBroadcaster>().AsSingle();
        }
    }
}