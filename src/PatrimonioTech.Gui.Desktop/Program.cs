using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PatrimonioTech.App.Credentials.v1.AddUser;
using PatrimonioTech.Gui.Desktop.DependencyInjection;

namespace PatrimonioTech.Gui.Desktop;

public class Program(ICredentialAddUserUseCase credentialAddUserUseCase)
{
    public async Task Run(string[] args)
    {
        await credentialAddUserUseCase.Execute(
            new CredentialAddUserRequest("Fabricio", "securepassword"),
            CancellationToken.None);

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task Main(string[] args)
    {
        var container = new DesktopContainer(LogLevel.Information);
        await container.GetRequiredService<Program>().Run(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
