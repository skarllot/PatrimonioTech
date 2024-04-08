using PatrimonioTech.Gui.Dashboard;
using PatrimonioTech.Gui.Login;
using PatrimonioTech.Gui.Users.Create;
using ReactiveUI;

namespace PatrimonioTech.Gui.DependencyInjection;

public sealed class ViewLocator : IViewLocator
{
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
    {
        LoginViewModel => new LoginView(),
        UserCreateViewModel => new UserCreateView(),
        DashboardViewModel => new DashboardView(),
        _ => null
    };
}
