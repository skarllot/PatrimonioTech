using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using PatrimonioTech.Gui.DependencyInjection;
using PatrimonioTech.Gui.Desktop.DependencyInjection;

namespace PatrimonioTech.Gui.Desktop;

public static class Program
{
    private static DesktopContainer? s_container;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            return BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            s_container?.Dispose();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
    {
        s_container ??= new DesktopContainer();
        return AppBuilder.Configure(s_container.GetRequiredService<IFactory<App>>().Create)
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}
