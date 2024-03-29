﻿using Generator.Equals;
using PatrimonioTech.Domain.Common.Parsers;

namespace PatrimonioTech.Domain.Common.ValueObjects;

[Equatable]
public sealed partial class Cnpj
{
    private const int Length = 14;

    private Cnpj(string value) => Value = value;

    public string Value { get; }

    public static Result<Cnpj, CnpjError> Create(string value)
    {
        return StringParser.NotNullOrWhitespace(value).ToResult(CnpjError.Empty)
            .Ensure(v => v.Length >= Length, CnpjError.TooShort)
            .Bind(v => Parser.TryNormalize(v))
            .Ensure(Parser.IsValid, CnpjError.Invalid)
            .Map(v => new Cnpj(v));
    }
}

public enum CnpjError
{
    Empty = 1,
    TooShort,
    TooLong,
    Invalid
}
