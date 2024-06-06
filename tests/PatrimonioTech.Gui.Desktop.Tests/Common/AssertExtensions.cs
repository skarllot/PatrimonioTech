using NetArchTest.Rules;

namespace PatrimonioTech.Gui.Desktop.Tests.Common;

public static class AssertExtensions
{
    public static TestResultAssertion Should(this TestResult result) => new(result);
}
