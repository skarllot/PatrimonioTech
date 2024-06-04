namespace PatrimonioTech.Domain.Common.Parsers;

public static class StringParser
{
    public static Option<string> NotNullOrWhitespace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? None : Some(value);

    public static Option<string> HaveLength(string value, Func<int, bool> countPredicate) =>
        countPredicate(value.Length) ? Some(value) : None;
}
