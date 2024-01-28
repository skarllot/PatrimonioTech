namespace PatrimonioTech.Domain.Common.Parsers;

public static class StringParser
{
    public static Maybe<string> NotNullOrWhitespace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? Maybe.None : Maybe.From(value);

    public static Maybe<string> HaveLength(string value, Func<int, bool> countPredicate) =>
        countPredicate(value.Length) ? Maybe.From(value) : Maybe.None;
}
