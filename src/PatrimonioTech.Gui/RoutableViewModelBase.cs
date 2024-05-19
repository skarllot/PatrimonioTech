using System.Collections;
using System.ComponentModel;
using PatrimonioTech.Gui.Common;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace PatrimonioTech.Gui;

public class RoutableViewModelBase : ViewModelBase, IRoutableViewModel, INotifyDataErrorInfo, IValidatableViewModel
{
    private readonly ReactiveValidationSubject _validationSubject = new();

    protected RoutableViewModelBase(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    event EventHandler<DataErrorsChangedEventArgs>? INotifyDataErrorInfo.ErrorsChanged
    {
        add => _validationSubject.ErrorsChanged += value;
        remove => _validationSubject.ErrorsChanged -= value;
    }

    public IScreen HostScreen { get; }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];

    bool INotifyDataErrorInfo.HasErrors => _validationSubject.HasErrors;

    public ValidationContext ValidationContext => _validationSubject.ValidationContext;

    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => _validationSubject.GetErrors(propertyName);
}
