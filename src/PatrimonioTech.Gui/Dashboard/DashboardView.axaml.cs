using Avalonia.ReactiveUI;
using ReactiveUI;

namespace PatrimonioTech.Gui.Dashboard;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}

