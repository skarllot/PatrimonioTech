﻿using FluentAssertions;
using PatrimonioTech.Domain.Common;
using Vogen;

namespace PatrimonioTech.Domain.Tests.Common;

public class CnpjTests
{
    [Theory]
    [InlineData("33.000.167/0577-23", "33000167057723")]
    [InlineData("33000167/0577-23", "33000167057723")]
    [InlineData("330001670577-23", "33000167057723")]
    [InlineData("33000167057723", "33000167057723")]
    [InlineData("00.038.166/0002-88", "00038166000288")]
    public void From_WithValidInput_ReturnsInstance(string input, string expected)
    {
        Cnpj cnpj = Cnpj.From(input);

        cnpj.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("33.000.168/0577-23")]
    [InlineData("32000167/0577-23")]
    [InlineData("330001670577-24")]
    [InlineData("33000177057723")]
    [InlineData("10.038.166/0002-88")]
    public void From_WithInvalidInput_ThrowsException(string input)
    {
        Action from = () => Cnpj.From(input);

        from.Should().ThrowExactly<ValueObjectValidationException>();
    }
}
