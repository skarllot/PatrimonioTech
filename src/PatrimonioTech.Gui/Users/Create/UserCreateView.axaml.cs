using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PatrimonioTech.Gui.Users.Create;

public sealed partial class UserCreateView : ReactiveUserControl<UserCreateViewModel>
{
    public UserCreateView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            ViewModel!.ShowError
                .RegisterHandler(ctx => ctx.SetOutput(Unit.Default))
                .DisposeWith(d);
        });
    }
}
