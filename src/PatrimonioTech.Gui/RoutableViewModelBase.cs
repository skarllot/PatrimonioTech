using ReactiveUI;

namespace PatrimonioTech.Gui;

public class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    protected RoutableViewModelBase(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public IScreen HostScreen { get; }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];
}
