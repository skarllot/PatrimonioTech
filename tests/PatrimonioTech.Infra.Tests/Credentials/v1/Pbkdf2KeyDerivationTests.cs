using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;
using PatrimonioTech.Infra.Credentials.Services;
using PatrimonioTech.Infra.Tests.Common;
using Xunit.Abstractions;

namespace PatrimonioTech.Infra.Tests.Credentials.v1;

public class Pbkdf2KeyDerivationTests(ITestOutputHelper outputHelper)
{
    private readonly Pbkdf2KeyDerivation _keyDerivation = new(NullLogger<Pbkdf2KeyDerivation>.Instance);

    public static TheoryData<string, int, int> ValidInputs =>
        (from p in new[] { "password", "Ovyr/X3uT6$>}ZM/(o'O'.37*O$*=nHn{8khQ_o6n}?|~}ITH<" }
            from k in new[] { 128, 256, 512, 1024 }
            from i in new[] { 1000, 10_000, 100_000 }
            select (p, k, i))
        .ToTheoryData();

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void CreateKey_WithValidInput_ReturnsSaltAndEncryptedKey(string password, int keySize, int iterations)
    {
        //Act
        var result = from p in Password.Create(password)
            select _keyDerivation.CreateKey(p, keySize, iterations);

        // Assert
        var (salt, encryptedKey) = result.Should().BeOk();

        salt.Should().NotBeEmpty();
        encryptedKey.Should().NotBeEmpty();

        outputHelper.WriteLine("Salt: {0}", salt);
        outputHelper.WriteLine("Encrypted key: {0}", encryptedKey);
    }

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void TryGetKey_WithValidInput_ReturnsKey(string password, int keySize, int iterations)
    {
        // Arrange
        var (salt, encryptedKey) = (from p in Password.Create(password)
                select _keyDerivation.CreateKey(p, keySize, iterations))
            .Unwrap();

        // Act
        var result1 = _keyDerivation.TryGetKey(password, salt, encryptedKey, keySize, iterations);
        var result2 = _keyDerivation.TryGetKey(password, salt, encryptedKey, keySize, iterations);

        // Assert
        result1.Should().BeOk();
        result1.Unwrap().Should().NotBeEmpty();
        result2.Unwrap().Should().Be(result1.Unwrap());

        outputHelper.WriteLine("Salt: {0}", salt);
        outputHelper.WriteLine("Encrypted key: {0}", encryptedKey);
        outputHelper.WriteLine("Key: {0}", result1.Unwrap());
    }

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void TryGetKey_WithInvalidSalt_ReturnsFailure(string password, int keySize, int iterations)
    {
        // Arrange
        const string salt = "invalid salt";
        const string encryptedKey = "encryptedKey";

        // Act
        var result = _keyDerivation.TryGetKey(password, salt, encryptedKey, keySize, iterations);

        // Assert
        result.Should().BeErr()
            .Should().Be(GetKeyError.InvalidSalt);
    }

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void TryGetKey_WithInvalidEncryptedKey_ReturnsFailure(string password, int keySize, int iterations)
    {
        // Arrange
        var (salt, encryptedKey) = (from p in Password.Create(password)
                select _keyDerivation.CreateKey(p, keySize, iterations))
            .Unwrap();

        // Act
        var result = _keyDerivation.TryGetKey(password, salt, encryptedKey[..5], keySize, iterations);

        // Assert
        result.Should().BeErr()
            .Should().Be(GetKeyError.InvalidEncryptedKey);
    }

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void TryGetKey_WithInvalidPassword_ReturnsFailure(string password, int keySize, int iterations)
    {
        // Arrange
        var (salt, encryptedKey) = (from p in Password.Create(password)
                select _keyDerivation.CreateKey(p, keySize, iterations))
            .Unwrap();

        // Act
        var result1 = _keyDerivation.TryGetKey(password[..^2], salt, encryptedKey, keySize, iterations);
        var result2 = _keyDerivation.TryGetKey(password + password, salt, encryptedKey, keySize, iterations);

        // Assert
        result1.Should().BeErr()
            .Should().Be(GetKeyError.InvalidPassword);
        result2.Should().BeErr()
            .Should().Be(GetKeyError.InvalidPassword);
    }
}
