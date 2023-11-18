using Vogen;

namespace PatrimonioTech.Domain.Common;

[ValueObject<string>(comparison: ComparisonGeneration.Omit)]
public readonly partial struct NotEmptyString
{
    private static string? NormalizeInput(string? value) => value?.Trim();

    private static Validation Validate(string value) => string.IsNullOrWhiteSpace(value)
        ? Validation.Invalid("Length must be greater than zero")
        : Validation.Ok;
}
