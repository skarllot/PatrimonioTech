using PatrimonioTech.Gui.Dashboard;
using PatrimonioTech.Gui.Login;
using PatrimonioTech.Gui.Users.Create;
using ReactiveUI;

namespace PatrimonioTech.Gui.DependencyInjection;

public sealed class ViewLocator : IViewLocator
{
    private static readonly Dictionary<Type, Func<IViewFor>> s_viewResolvers = new()
    {
        { typeof(LoginViewModel), static () => new LoginView() },
        { typeof(UserCreateViewModel), static () => new UserCreateView() },
        { typeof(DashboardViewModel), static () => new DashboardView() },
    };

    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null) where TViewModel : class =>
        s_viewResolvers.GetValueOrDefault(typeof(TViewModel))?.Invoke() as IViewFor<TViewModel>;

    public IViewFor? ResolveView(object? instance, string? contract = null) => instance is not null
        ? s_viewResolvers.GetValueOrDefault(instance.GetType())?.Invoke()
        : null;
}
