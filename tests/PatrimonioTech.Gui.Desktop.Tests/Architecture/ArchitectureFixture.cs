using System.Reflection;

namespace PatrimonioTech.Gui.Desktop.Tests.Architecture;

public static class ArchitectureFixture
{
    public const string BaseNamespace = nameof(PatrimonioTech);

    public static Assembly DomainAssembly { get; } = Assembly.Load($"{BaseNamespace}.Domain");
    public static Assembly AppAssembly { get; } = Assembly.Load($"{BaseNamespace}.App");
    public static Assembly InfraAssembly { get; } = Assembly.Load($"{BaseNamespace}.Infra");
    public static Assembly GuiAssembly { get; } = Assembly.Load($"{BaseNamespace}.Gui");
    public static Assembly GuiDesktopAssembly { get; } = Assembly.Load($"{BaseNamespace}.Gui.Desktop");

    public static IEnumerable<Assembly> AppAndDepsAssemblies { get; } = [DomainAssembly, AppAssembly];
    public static IEnumerable<Assembly> InfraAndDepsAssemblies { get; } = [..AppAndDepsAssemblies, InfraAssembly];

    public static IEnumerable<Assembly> AllAssemblies { get; } =
        [DomainAssembly, AppAssembly, InfraAssembly, GuiAssembly, GuiDesktopAssembly];
}
