using Avalonia.ReactiveUI;
using ReactiveUI;

namespace PatrimonioTech.Gui.Users.Create;

public partial class UserCreateView : ReactiveUserControl<UserCreateViewModel>
{
    public UserCreateView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
