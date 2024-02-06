using Avalonia.ReactiveUI;
using ReactiveUI;

namespace PatrimonioTech.Gui.Main;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
