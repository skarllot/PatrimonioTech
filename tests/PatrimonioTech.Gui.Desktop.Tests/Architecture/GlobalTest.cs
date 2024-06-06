using NetArchTest.Rules;
using static PatrimonioTech.Gui.Desktop.Tests.Architecture.ArchitectureFixture;

namespace PatrimonioTech.Gui.Desktop.Tests.Architecture;

public class GlobalTest
{
    [Fact]
    public void EnsureCorrectDependencyBetweenLayers()
    {
        Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                $"{BaseNamespace}.App",
                $"{BaseNamespace}.Infra",
                $"{BaseNamespace}.Gui")
            .GetResult().Should().Succeed();

        Types
            .InAssembly(AppAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                $"{BaseNamespace}.Infra",
                $"{BaseNamespace}.Gui")
            .GetResult().Should().Succeed();

        Types
            .InAssembly(InfraAssembly)
            .Should()
            .NotHaveDependencyOnAny($"{BaseNamespace}.Gui")
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureOnlyDependencyContainerDependsOnJab()
    {
        Types
            .InAssemblies(AllAssemblies)
            .That()
            .DoNotResideInNamespaceContaining("DependencyInjection")
            .Should()
            .NotHaveDependencyOnAny("Jab")
            .GetResult().Should().Succeed();
    }
}
