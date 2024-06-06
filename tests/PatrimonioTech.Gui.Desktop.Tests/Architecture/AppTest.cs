using NetArchTest.Rules;
using static PatrimonioTech.Gui.Desktop.Tests.Architecture.ArchitectureFixture;

namespace PatrimonioTech.Gui.Desktop.Tests.Architecture;

public class AppTest
{
    [Fact]
    public void EnsureNoTypesAtRootLevel()
    {
        Types
            .InAssembly(AppAssembly)
            .Should()
            .NotResideInNamespaceMatching($"{BaseNamespace}[.]App$")
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureUseCaseRequestAndResponseAreVersioned()
    {
        Types
            .InAssembly(AppAssembly)
            .That()
            .HaveNameEndingWith("UseCase", "Request", "Response", "Error")
            .And().DoNotResideInNamespaceEndingWith(".Database")
            .Should()
            .ResideInNamespaceMatching(@"[.]v\d+[.]")
            .GetResult().Should().Succeed();
    }
}
