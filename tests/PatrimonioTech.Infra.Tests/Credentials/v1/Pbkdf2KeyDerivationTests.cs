using FluentAssertions;
using FluentAssertions.LanguageExt;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;
using PatrimonioTech.Infra.Credentials.Services;
using PatrimonioTech.Infra.Tests.Common;
using Xunit.Abstractions;

namespace PatrimonioTech.Infra.Tests.Credentials.v1;

public class Pbkdf2KeyDerivationTests(ITestOutputHelper outputHelper)
{
    private readonly Pbkdf2KeyDerivation _keyDerivation = new();

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
        (string? salt, string? encryptedKey) = result.Should().BeRight().Subject;

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
        (string salt, string encryptedKey) = (from p in Password.Create(password)
                select _keyDerivation.CreateKey(p, keySize, iterations))
            .First().Right;

        // Act
        var result1 = _keyDerivation.TryGetKey(password, salt, encryptedKey, keySize, iterations);
        var result2 = _keyDerivation.TryGetKey(password, salt, encryptedKey, keySize, iterations);

        // Assert
        result1.Should().BeRight();
        result1.IfLeft("").Should().NotBeEmpty();
        result2.IfLeft("").Should().Be(result1.IfLeft(""));

        outputHelper.WriteLine("Salt: {0}", salt);
        outputHelper.WriteLine("Encrypted key: {0}", encryptedKey);
        outputHelper.WriteLine("Key: {0}", result1.IfLeft(""));
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
        result.Should().BeLeft()
            .Which.Value.Should().BeOfType<GetKeyError.InvalidSalt>();
    }

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void TryGetKey_WithInvalidEncryptedKey_ReturnsFailure(string password, int keySize, int iterations)
    {
        // Arrange
        (string salt, string encryptedKey) = (from p in Password.Create(password)
                select _keyDerivation.CreateKey(p, keySize, iterations))
            .First().Right;

        // Act
        var result = _keyDerivation.TryGetKey(password, salt, encryptedKey[..5], keySize, iterations);

        // Assert
        result.Should().BeLeft()
            .Which.Value.Should().BeOfType<GetKeyError.InvalidEncryptedKey>();
    }
}
