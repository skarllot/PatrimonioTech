using Avalonia.Controls;
using Avalonia.Controls.Templates;
using PatrimonioTech.Gui.ViewModels;

namespace PatrimonioTech.Gui;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? param)
    {
        string? name = param?.GetType().FullName!.Replace("ViewModel", "View");
        var type = name is not null ? Type.GetType(name) : null;

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}