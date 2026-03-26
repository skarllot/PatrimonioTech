using FluentAssertions;
using JetBrains.Annotations;
using PatrimonioTech.Domain.Credentials;

namespace PatrimonioTech.Domain.Tests.Credentials;

[TestSubject(typeof(Password))]
public class PasswordTests
{
    [Fact]
    public void PasswordMinLength_Is8()
    {
        Password.PasswordMinLength.Should().Be(8);
    }

    [Fact]
    public void Create_WithNull_ReturnsEmptyError()
    {
        var result = Password.Create(null!);

        result.Should().BeErr().Should().Be(PasswordError.Empty);
    }

    [Fact]
    public void Create_WithEmptyString_ReturnsEmptyError()
    {
        var result = Password.Create("");

        result.Should().BeErr().Should().Be(PasswordError.Empty);
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ReturnsEmptyError()
    {
        var result = Password.Create("   ");

        result.Should().BeErr().Should().Be(PasswordError.Empty);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1234567")]
    public void Create_WithFewerThan8Characters_ReturnsTooShortError(string value)
    {
        var result = Password.Create(value);

        result.Should().BeErr().Should().Be(PasswordError.TooShort);
    }

    [Fact]
    public void Create_With8Characters_ReturnsOk()
    {
        var result = Password.Create("12345678");

        result.Should().BeOk();
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("averylongpassword")]
    public void Create_WithMoreThan8Characters_ReturnsOk(string value)
    {
        var result = Password.Create(value);

        result.Should().BeOk();
    }
}
