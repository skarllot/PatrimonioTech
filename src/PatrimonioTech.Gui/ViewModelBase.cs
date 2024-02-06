using System.ComponentModel;
using ReactiveUI;

namespace PatrimonioTech.Gui;

public class ViewModelBase : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    protected virtual void OnPropertyChanging(PropertyChangingEventArgs eventArgs)
    {
        this.RaisePropertyChanging(eventArgs.PropertyName);
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        this.RaisePropertyChanged(eventArgs.PropertyName);
    }
}
