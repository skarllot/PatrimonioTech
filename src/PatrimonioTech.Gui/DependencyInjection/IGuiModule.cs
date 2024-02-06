using Jab;
using PatrimonioTech.Gui.Login;
using PatrimonioTech.Gui.Main;
using ReactiveUI;

namespace PatrimonioTech.Gui.DependencyInjection;

[ServiceProviderModule]
[Transient<App>]
[Transient<MainWindow>(Factory = nameof(GetMainWindow))]
[Singleton<IFactory<App>, ContainerFactory<App>>]
[Singleton<IFactory<MainWindow>, ContainerFactory<MainWindow>>]

// View Models
[Singleton<MainWindowViewModel>]
[Transient<IScreen>(Factory = nameof(GetScreen))]
[Transient<LoginViewModel>]
[Singleton<IFactory<LoginViewModel>, ContainerFactory<LoginViewModel>>]
public interface IGuiModule
{
    public static MainWindow GetMainWindow(MainWindowViewModel viewModel) => new() { ViewModel = viewModel };
    public static IScreen GetScreen(MainWindowViewModel vm) => vm;
}
