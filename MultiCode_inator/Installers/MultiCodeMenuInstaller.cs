using MultiCode_inator.Managers;
using MultiCode_inator.UI.FlowCoordinators;
using MultiCode_inator.UI.ViewControllers;
using Zenject;

namespace MultiCode_inator.Installers
{
    internal class MultiCodeMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
            Container.Bind<MultiCodeFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<MultiCodeSettingsViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesTo<CommandToggleModalViewController>().AsSingle();
            Container.Bind<GitHubPageModalController>().AsSingle();
        }
    }
}