using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PatrimonioTech.App.SelfApplication;
using PatrimonioTech.Gui.DependencyInjection;
using PatrimonioTech.Gui.Main;

namespace PatrimonioTech.Gui;

public class App(
    ILocalPathProvider localPathProvider,
    IFactory<MainWindow> mainWindow)
    : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        localPathProvider.Initialize();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow.Create();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
