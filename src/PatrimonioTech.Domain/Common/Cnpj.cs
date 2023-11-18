using Vogen;

namespace PatrimonioTech.Domain.Common;

[ValueObject<string>(comparison: ComparisonGeneration.Omit)]
public readonly partial struct Cnpj
{
    private const int Length = 14;

    private static string? NormalizeInput(string? value)
    {
        return Parser.TryNormalize(value, out string? result) ? result : value;
    }

    private static Validation Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Validation.Invalid("Length must be greater than zero");
        if (input.Length < Length)
            return Validation.Invalid("Length is too short");
        if (input.Length > Length)
            return Validation.Invalid("Length is too long");
        if (!Parser.IsValid(input))
            return Validation.Invalid("Invalid value");

        return Validation.Ok;
    }
}
