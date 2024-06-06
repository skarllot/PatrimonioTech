using Avalonia.ReactiveUI;
using ReactiveUI;

namespace PatrimonioTech.Gui.Dashboard;

public sealed partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}

