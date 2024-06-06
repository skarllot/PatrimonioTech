using System.Diagnostics;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using NetArchTest.Rules;

namespace PatrimonioTech.Gui.Desktop.Tests.Common;

[DebuggerNonUserCode]
public class TestResultAssertion(TestResult subject)
    : ReferenceTypeAssertions<TestResult, TestResultAssertion>(subject)
{
    protected override string Identifier => "TestResult";

    public AndConstraint<TestResultAssertion> Succeed(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .ForCondition(Subject.IsSuccessful)
            .BecauseOf(because, becauseArgs)
            .FailWith(
                "Expected test result to succeed{reason}, but it has those errors:{0}",
                GetErrors);

        Execute.Assertion
            .ForCondition(Subject.SelectedTypesForTesting.Any())
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected test result to select at least one type{reason}");

        return new AndConstraint<TestResultAssertion>(this);
    }

    private string GetErrors() => Subject.FailingTypes.Aggregate(
        string.Empty,
        (s, t) => $"{s}\n{t.FullName}: {t.Explanation ?? "<undefined explanation>"}");
}
