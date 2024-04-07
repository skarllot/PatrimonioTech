using Jab;
using PatrimonioTech.Gui.Dashboard;
using PatrimonioTech.Gui.Login;
using PatrimonioTech.Gui.Main;
using ReactiveUI;

namespace PatrimonioTech.Gui.DependencyInjection;

[ServiceProviderModule]
[Singleton(typeof(IFactory<>), typeof(ContainerFactory<>))]
[Transient<App>]
[Transient<MainWindow>(Factory = nameof(GetMainWindow))]
[Singleton<IFactory<App>, ContainerFactory<App>>]

// View Models
[Singleton<MainWindowViewModel>]
[Transient<IScreen>(Factory = nameof(GetScreen))]
[Transient<LoginViewModel>]
[Transient<DashboardViewModel>]
public interface IGuiModule
{
    public static MainWindow GetMainWindow(MainWindowViewModel viewModel) => new() { ViewModel = viewModel };
    public static IScreen GetScreen(MainWindowViewModel vm) => vm;
}
