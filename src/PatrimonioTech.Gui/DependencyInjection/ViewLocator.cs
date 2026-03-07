using PatrimonioTech.Gui.Dashboard;
using PatrimonioTech.Gui.Login;
using PatrimonioTech.Gui.Users.Create;
using ReactiveUI;

namespace PatrimonioTech.Gui.DependencyInjection;

public sealed class ViewLocator : IViewLocator
{
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null) where TViewModel : class
    {
        if (typeof(TViewModel) == typeof(LoginViewModel))
        {
            return (IViewFor<TViewModel>)(IViewFor)new LoginView();
        }

        if (typeof(TViewModel) == typeof(UserCreateViewModel))
        {
            return (IViewFor<TViewModel>)(IViewFor)new UserCreateView();
        }

        if (typeof(TViewModel) == typeof(DashboardViewModel))
        {
            return (IViewFor<TViewModel>)(IViewFor)new DashboardView();
        }

        return null;
    }

    public IViewFor? ResolveView(object? instance, string? contract = null) => instance switch
    {
        LoginViewModel => new LoginView(),
        UserCreateViewModel => new UserCreateView(),
        DashboardViewModel => new DashboardView(),
        _ => null,
    };
}
