using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;
using PatrimonioTech.Infra.Credentials.Services;
using Xunit.Abstractions;

namespace PatrimonioTech.Infra.Tests.Credentials.v1;

public class Pbkdf2KeyDerivationTests(ITestOutputHelper outputHelper)
{
    private readonly Pbkdf2KeyDerivation _keyDerivation = new(NullLogger<Pbkdf2KeyDerivation>.Instance);

    [Theory]
    [InlineData("password")]
    [InlineData("Ovyr/X3uT6$>}ZM/(o'O'.37*O$*=nHn{8khQ_o6n}?|~}ITH<")]
    public void CreateKey_WithValidPassword_ReturnsPhcString(string password)
    {
        var p = Password.Create(password).Unwrap();

        var result = _keyDerivation.CreateKey(p);

        var phcString = result.Should().BeOk();
        phcString.Value.Should().StartWith("$pbkdf2-sha512-aes256cbc$");

        outputHelper.WriteLine("PHC: {0}", phcString.Value);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("Ovyr/X3uT6$>}ZM/(o'O'.37*O$*=nHn{8khQ_o6n}?|~}ITH<")]
    public void TryGetKey_WithValidPasswordAndPhcString_ReturnsKey(string password)
    {
        var p = Password.Create(password).Unwrap();
        var phcString = _keyDerivation.CreateKey(p).Unwrap();

        var result1 = _keyDerivation.TryGetKey(password, phcString.Value);
        var result2 = _keyDerivation.TryGetKey(password, phcString.Value);

        result1.Should().BeOk();
        result1.Unwrap().Should().NotBeEmpty();
        result2.Unwrap().Should().Be(result1.Unwrap());

        outputHelper.WriteLine("PHC: {0}", phcString.Value);
        outputHelper.WriteLine("Key: {0}", result1.Unwrap());
    }

    [Theory]
    [InlineData("password")]
    [InlineData("Ovyr/X3uT6$>}ZM/(o'O'.37*O$*=nHn{8khQ_o6n}?|~}ITH<")]
    public void TryGetKey_WithInvalidPassword_ReturnsInvalidPassword(string password)
    {
        var p = Password.Create(password).Unwrap();
        var phcString = _keyDerivation.CreateKey(p).Unwrap();

        var result1 = _keyDerivation.TryGetKey(password[..^2], phcString.Value);
        var result2 = _keyDerivation.TryGetKey(password + password, phcString.Value);

        result1.Should().BeErr().Should().Be(GetKeyError.InvalidPassword);
        result2.Should().BeErr().Should().Be(GetKeyError.InvalidPassword);
    }

    [Fact]
    public void TryGetKey_WithMalformedPhcString_ReturnsInvalidHash()
    {
        var result = _keyDerivation.TryGetKey("password", "not-a-valid-phc-string");

        result.Should().BeErr().Should().Be(GetKeyError.InvalidHash);
    }
}
