using FluentAssertions;
using JetBrains.Annotations;
using NSubstitute;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Domain.Tests.Credentials.Actions.AddUser;

[TestSubject(typeof(AddUserScenario))]
public class AddUserScenarioTests
{
    private static readonly AddUserCredential ValidCommand = new("ValidUser", "password1");

    private readonly IKeyDerivation _keyDerivation = Substitute.For<IKeyDerivation>();
    private readonly AddUserScenario _sut;

    public AddUserScenarioTests()
    {
        _keyDerivation
            .CreateKey(Arg.Any<Password>())
            .Returns(new TestPhcString("$pbkdf2-sha512-aes256cbc$i=100000,l=512$salt$key"));
        _sut = new AddUserScenario(_keyDerivation);
    }

    [Fact]
    public void NameMinLength_Is3()
    {
        AddUserScenario.NameMinLength.Should().Be(3);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData("  a  ")]
    public void Execute_WithShortName_ReturnsNameTooShort(string name)
    {
        var command = ValidCommand with { Name = name };

        var result = _sut.Execute(command);

        result.Should().BeErr().Should().BeOfType<AddUserCredentialError.NameTooShort>();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("  abc  ")]
    [InlineData("ValidUser")]
    public void Execute_WithNameOfAtLeast3Chars_AcceptsName(string name)
    {
        var command = ValidCommand with { Name = name };

        var result = _sut.Execute(command);

        result.Should().BeOk();
    }

    [Fact]
    public void Execute_WithPaddedName_StoresTrimmedName()
    {
        var command = ValidCommand with { Name = "  ValidUser  " };

        var result = _sut.Execute(command);

        result.Should().BeOk().Name.Should().Be("ValidUser");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Execute_WithEmptyOrWhitespacePassword_ReturnsInvalidPasswordEmpty(string password)
    {
        var command = ValidCommand with { Password = password };

        var result = _sut.Execute(command);

        result.Should().BeErr().Should().BeOfType<AddUserCredentialError.InvalidPassword>()
            .Which.Error.Should().Be(PasswordError.Empty);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1234567")]
    public void Execute_WithPasswordShorterThan8Chars_ReturnsInvalidPasswordTooShort(string password)
    {
        var command = ValidCommand with { Password = password };

        var result = _sut.Execute(command);

        result.Should().BeErr().Should().BeOfType<AddUserCredentialError.InvalidPassword>()
            .Which.Error.Should().Be(PasswordError.TooShort);
    }

    [Fact]
    public void Execute_WithKeyDerivationFailure_ReturnsKeyDerivationFailed()
    {
        _keyDerivation.CreateKey(Arg.Any<Password>()).Returns(CryptographyError.KeyDerivationFailed);

        var result = _sut.Execute(ValidCommand);

        result.Should().BeErr().Should().BeOfType<AddUserCredentialError.KeyDerivationFailed>();
    }

    [Fact]
    public void Execute_WithValidInputs_CallsKeyDerivationAndReturnsOk()
    {
        var result = _sut.Execute(ValidCommand);

        result.Should().BeOk();
        _keyDerivation.Received(1).CreateKey(Arg.Any<Password>());
    }

    [Fact]
    public void Execute_WithValidInputs_ReturnsUserCredentialAddedWithExpectedData()
    {
        var result = _sut.Execute(ValidCommand);

        var added = result.Should().BeOk();
        added.Name.Should().Be(ValidCommand.Name);
        added.PasswordHash.Should().Be("$pbkdf2-sha512-aes256cbc$i=100000,l=512$salt$key");
        added.Database.Should().NotBe(Guid.Empty);
    }

    private sealed record TestPhcString(string Value) : IPhcString;
}
