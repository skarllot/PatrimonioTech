using FxKit.Parsers;
using Generator.Equals;

namespace PatrimonioTech.Domain.Common.ValueObjects;

[Equatable]
public sealed partial class Cnpj
{
    private const int Length = 14;

    private Cnpj(string value) => Value = value;

    public string Value { get; }

    public static Result<Cnpj, CnpjError> Create(string value)
    {
        return StringParser.NonNullOrWhiteSpace(value).OkOr(CnpjError.Empty)
            .Ensure(v => v.Length >= Length, CnpjError.TooShort)
            .FlatMap(v => Parser.TryNormalize(v))
            .Ensure(Parser.IsValid, CnpjError.Invalid)
            .Map(v => new Cnpj(v));
    }
}
