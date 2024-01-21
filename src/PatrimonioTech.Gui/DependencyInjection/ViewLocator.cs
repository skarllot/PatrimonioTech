using PatrimonioTech.Gui.Login;
using ReactiveUI;

namespace PatrimonioTech.Gui.DependencyInjection;

public sealed class ViewLocator : IViewLocator
{
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
    {
        LoginViewModel => new LoginView(),
        _ => null
    };
}
