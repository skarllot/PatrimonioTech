using Avalonia.ReactiveUI;
using ReactiveUI;

namespace PatrimonioTech.Gui.Login;

public sealed partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
