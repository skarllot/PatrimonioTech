using FluentAssertions;
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
        (string salt, string encryptedKey) = _keyDerivation.CreateKey(password, keySize, iterations);

        // Assert
        Assert.NotEqual(string.Empty, salt);
        Assert.NotEqual(string.Empty, encryptedKey);

        outputHelper.WriteLine("Salt: {0}", salt);
        outputHelper.WriteLine("Encrypted key: {0}", encryptedKey);
    }

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void TryGetKey_WithValidInput_ReturnsKey(string password, int keySize, int iterations)
    {
        // Arrange
        (string salt, string encryptedKey) = _keyDerivation.CreateKey(password, keySize, iterations);

        // Act
        var result1 = _keyDerivation.TryGetKey(password, salt, encryptedKey, keySize, iterations);
        var result2 = _keyDerivation.TryGetKey(password, salt, encryptedKey, keySize, iterations);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result1.IfFail("").Should().NotBeEmpty();
        result2.IfFail("").Should().Be(result1.IfFail(""));

        outputHelper.WriteLine("Salt: {0}", salt);
        outputHelper.WriteLine("Encrypted key: {0}", encryptedKey);
        outputHelper.WriteLine("Key: {0}", result1.IfFail(""));
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
        result.IsSuccess.Should().BeFalse();
        result.Match(_ => null, e => (string?)e.Message).Should().Be("Invalid salt");
    }

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void TryGetKey_WithInvalidEncryptedKey_ReturnsFailure(string password, int keySize, int iterations)
    {
        // Arrange
        (string salt, string encryptedKey) = _keyDerivation.CreateKey(password, keySize, iterations);

        // Act
        var result = _keyDerivation.TryGetKey(password, salt, encryptedKey[..5], keySize, iterations);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Match(_ => null, e => (string?)e.Message).Should().Be("Invalid encrypted key");
    }
}
