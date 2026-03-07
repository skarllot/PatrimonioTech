using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PatrimonioTech.Gui.Login;

public sealed partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
