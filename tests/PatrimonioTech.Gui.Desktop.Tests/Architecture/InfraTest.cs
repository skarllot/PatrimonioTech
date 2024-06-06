using FxKit.CompilerServices;
using NetArchTest.Rules;
using static PatrimonioTech.Gui.Desktop.Tests.Architecture.ArchitectureFixture;

namespace PatrimonioTech.Gui.Desktop.Tests.Architecture;

public class InfraTest
{
    [Fact]
    public void EnsureNoTypesAtRootLevel()
    {
        Types
            .InAssembly(InfraAssembly)
            .Should()
            .NotResideInNamespaceMatching($"{BaseNamespace}[.]Infra$")
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureRepositoriesDependsOnDomain()
    {
        Types
            .InAssembly(InfraAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .HaveDependencyOnAll($"{BaseNamespace}.Domain")
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureInfraAndDepsObjectsAreSealed()
    {
        Types
            .InAssemblies(InfraAndDepsAssemblies)
            .That()
            .AreClasses().And().DoNotHaveCustomAttribute<UnionAttribute>()
            .Should()
            .BeSealed().Or().BeStatic()
            .GetResult().Should().Succeed();
    }
}
