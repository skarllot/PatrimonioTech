using System.Reactive.Disposables;
using ReactiveUI;

namespace PatrimonioTech.Gui.Dashboard;

public class DashboardViewModel : RoutableViewModelBase
{
    public DashboardViewModel(IScreen hostScreen)
        : base(hostScreen)
    {
        this.WhenActivated((CompositeDisposable _) => { });
    }
}
