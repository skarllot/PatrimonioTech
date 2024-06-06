using Mono.Cecil;
using NetArchTest.Rules;

namespace PatrimonioTech.Gui.Desktop.Tests.Common;

public class ViewAndViewModelSameNamespaceRule : ICustomRule2
{
    public CustomRuleResult MeetsRule(TypeDefinition type)
    {
        if (type.BaseType is not GenericInstanceType baseType ||
            !string.Equals(baseType.Name, "ReactiveUserControl`1", StringComparison.Ordinal))
        {
            return new CustomRuleResult(isMet: false, "View does not inherit from ReactiveUserControl");
        }

        var viewModelType = baseType.GenericArguments[0];

        if (!string.Equals(viewModelType.Namespace, type.Namespace, StringComparison.Ordinal))
        {
            return new CustomRuleResult(isMet: false, "View and view model does not reside on same namespace");
        }

        return new CustomRuleResult(isMet: true);
    }
}
