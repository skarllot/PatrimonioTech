using System.Text.RegularExpressions;
using Vogen;
using Validation = Vogen.Validation;

namespace PatrimonioTech.Domain.Ativos;

[ValueObject<string>(comparison: ComparisonGeneration.Omit)]
public readonly partial struct B3TickerName
{
    [GeneratedRegex("^[A-Z0-9]{4}$")]
    private static partial Regex GetValidationPattern();

    private static string? NormalizeInput(string? value)
    {
        var inputTrimmed = value.AsSpan().Trim();
        if (inputTrimmed.Length != 4)
            return value;

        Span<char> result = stackalloc char[4];
        inputTrimmed.ToUpperInvariant(result);
        return result.ToString();
    }

    private static Validation Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Validation.Invalid("Length must be greater than zero");
        if (!GetValidationPattern().IsMatch(input))
            return Validation.Invalid("Invalid value");

        return Validation.Ok;
    }
}
