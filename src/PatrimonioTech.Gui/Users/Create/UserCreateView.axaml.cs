using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PatrimonioTech.Gui.Users.Create;

public sealed partial class UserCreateView : ReactiveUserControl<UserCreateViewModel>
{
    public UserCreateView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
