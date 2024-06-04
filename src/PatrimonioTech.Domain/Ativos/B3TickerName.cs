using System.Text.RegularExpressions;
using Generator.Equals;
using PatrimonioTech.Domain.Common.Parsers;

namespace PatrimonioTech.Domain.Ativos;

[Equatable]
public sealed partial class B3TickerName
{
    private B3TickerName(string value) => Value = value;

    public string Value { get; }

    [GeneratedRegex("^[A-Z0-9]{4}$", RegexOptions.Compiled, 150)]
    private static partial Regex GetValidationPattern();

    public static Result<B3TickerName, B3TickerNameError> Create(string value)
    {
        return StringParser.NotNullOrWhitespace(value).OkOrElse(B3TickerNameError.Empty)
            .Ensure(v => GetValidationPattern().IsMatch(value), B3TickerNameError.Invalid)
            .Map(v => new B3TickerName(v));
    }

    public override string ToString() => Value;
}
