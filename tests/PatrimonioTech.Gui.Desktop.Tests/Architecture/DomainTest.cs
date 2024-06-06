using NetArchTest.Rules;
using static PatrimonioTech.Gui.Desktop.Tests.Architecture.ArchitectureFixture;

namespace PatrimonioTech.Gui.Desktop.Tests.Architecture;

public class DomainTest
{
    [Fact]
    public void EnsureNoTypesAtRootLevel()
    {
        Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotResideInNamespaceMatching($"{BaseNamespace}[.]Domain$")
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureScenariosAreInDomainActionsNamespace()
    {
        Types
            .InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Scenario")
            .Should()
            .ResideInNamespaceMatching(@"[.]Actions[.]\w+$")
            .GetResult().Should().Succeed();

        Types
            .InAssemblies(AllAssemblies.Except([DomainAssembly]))
            .Should()
            .NotHaveNameEndingWith("Scenario")
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureRepositoriesInterfacesAreInServicesNamespace()
    {
        Types
            .InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .BeInterfaces()
            .And().ResideInNamespaceEndingWith("Services")
            .GetResult().Should().Succeed();
    }
}
