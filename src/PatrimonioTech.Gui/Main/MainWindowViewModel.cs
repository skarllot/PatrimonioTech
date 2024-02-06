using System.Reactive.Disposables;
using PatrimonioTech.Gui.DependencyInjection;
using PatrimonioTech.Gui.Login;
using ReactiveUI;

namespace PatrimonioTech.Gui.Main;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public MainWindowViewModel(IFactory<LoginViewModel> login)
    {
        this.WhenActivated(
            disposables =>
            {
                Router.Navigate
                    .Execute(login.Create())
                    .Subscribe()
                    .DisposeWith(disposables);
            });
    }

    public RoutingState Router { get; } = new();
}
