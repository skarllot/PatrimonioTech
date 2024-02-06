using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PatrimonioTech.Gui.DependencyInjection;
using PatrimonioTech.Gui.Main;

namespace PatrimonioTech.Gui;

public partial class App(IFactory<MainWindow> mainWindow) : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow.Create();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
