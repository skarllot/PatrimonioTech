using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using PatrimonioTech.Gui.Desktop.DependencyInjection;

namespace PatrimonioTech.Gui.Desktop;

public class Program(IFactory<App> appFactory)
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        return new DesktopContainer()
            .GetRequiredService<Program>()
            .Run(args);
    }

    public int Run(string[] args) =>
        BuildAvaloniaApp(appFactory)
            .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp(IFactory<App> appFactory)
        => AppBuilder.Configure(appFactory.Create)
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
