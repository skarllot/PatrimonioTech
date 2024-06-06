using Avalonia.ReactiveUI;
using NetArchTest.Rules;
using ReactiveUI;
using static PatrimonioTech.Gui.Desktop.Tests.Architecture.ArchitectureFixture;

namespace PatrimonioTech.Gui.Desktop.Tests.Architecture;

public class GuiTest
{
    [Fact]
    public void EnsureAppIsAtRootNamespace()
    {
        Types
            .InAssembly(GuiAssembly)
            .That()
            .HaveName("App")
            .Should()
            .ResideInNamespace($"{BaseNamespace}.Gui")
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureViewsImplementsReactiveUserControl()
    {
        Types
            .InAssembly(GuiAssembly)
            .That()
            .HaveNameEndingWith("View")
            .Should()
            .Inherit(typeof(ReactiveUserControl<>))
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureViewModelsImplementsRoutableViewModelBase()
    {
        Types
            .InAssembly(GuiAssembly)
            .That()
            .HaveNameEndingWith("ViewModel")
            .Should()
            .Inherit<RoutableViewModelBase>()
            .Or().Inherit<ViewModelBase>().And().ImplementInterface<IScreen>()
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureViewsAndViewModelsAreSameNamespace()
    {
        Types
            .InAssembly(GuiAssembly)
            .That()
            .HaveNameEndingWith("View")
            .Should()
            .MeetCustomRule(new ViewAndViewModelSameNamespaceRule())
            .GetResult().Should().Succeed();
    }

    [Fact]
    public void EnsureViewsAndViewModelsAreSealedClasses()
    {
        Types
            .InAssembly(GuiAssembly)
            .That()
            .HaveNameEndingWith("View", "ViewModel")
            .Should()
            .BeClasses()
            .And().BeSealed()
            .GetResult().Should().Succeed();
    }
}
