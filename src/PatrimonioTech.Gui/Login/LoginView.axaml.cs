using Avalonia.ReactiveUI;
using ReactiveUI;

namespace PatrimonioTech.Gui.Login;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
