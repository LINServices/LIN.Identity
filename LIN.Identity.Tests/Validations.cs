using LIN.Cloud.Identity.Services.Formats;
using LIN.Types.Cloud.Identity.Enumerations;
using LIN.Types.Cloud.Identity.Models;

namespace LIN.Identity.Tests;

public class AccountTests
{

    [Fact]
    public void Validate_ShouldReturnFalse_WhenNameIsEmpty()
    {
        // Arrange
        var account = new AccountModel
        {
            Name = "",
            Identity = new IdentityModel { Unique = "unique_user" }
        };

        // Act
        var result = Account.Validate(account);

        // Assert
        Assert.False(result.pass);
        Assert.Equal("La cuenta debe de tener un nombre valido.", result.message);
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenIdentityIsNull()
    {
        // Arrange
        var account = new AccountModel
        {
            Name = "Test Account",
            Identity = null
        };

        // Act
        var result = Account.Validate(account);

        // Assert
        Assert.False(result.pass);
        Assert.Equal("La cuenta debe de tener una identidad unica valida.", result.message);
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenIdentityUniqueIsEmpty()
    {
        // Arrange
        var account = new AccountModel
        {
            Name = "Test Account",
            Identity = new IdentityModel { Unique = "" }
        };

        // Act
        var result = Account.Validate(account);

        // Assert
        Assert.False(result.pass);
        Assert.Equal("La cuenta debe de tener una identidad unica valida.", result.message);
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenIdentityUniqueContainsNonAlphanumeric()
    {
        // Arrange
        var account = new AccountModel
        {
            Name = "Test Account",
            Identity = new IdentityModel { Unique = "unique_user!" }
        };

        // Act
        var result = Account.Validate(account);

        // Assert
        Assert.False(result.pass);
        Assert.Equal("La identidad de la cuenta no puede contener símbolos NO alfanuméricos.", result.message);
    }

    [Fact]
    public void Validate_ShouldReturnTrue_WhenAccountIsValid()
    {
        // Arrange
        var account = new AccountModel
        {
            Name = "Test Account",
            Identity = new IdentityModel { Unique = "uniqueuser" }
        };

        // Act
        var result = Account.Validate(account);

        // Assert
        Assert.True(result.pass);
        Assert.Equal("", result.message);
    }

    [Fact]
    public void Process_ShouldReturnProcessedAccountModel()
    {
        // Arrange
        var account = new AccountModel
        {
            Name = " Test Account ",
            Profile = new byte[] { 0x01, 0x02 },
            Password = "password",
            Visibility = Visibility.Visible,
            Identity = new IdentityModel
            {
                EffectiveTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddYears(1),
                Unique = "uniqueuser"
            }
        };

        // Act
        var processedAccount = Account.Process(account);

        // Assert
        Assert.Equal("Test Account", processedAccount.Name);
        Assert.Equal(account.Profile, processedAccount.Profile);
        Assert.NotEqual("password", processedAccount.Password); // Assuming encryption changes the password
        Assert.Equal(Visibility.Visible, processedAccount.Visibility);
        Assert.Equal("uniqueuser", processedAccount.Identity.Unique);
        Assert.Equal(IdentityStatus.Enable, processedAccount.Identity.Status);
        Assert.Equal(IdentityType.Account, processedAccount.Identity.Type);
        Assert.Equal(account.Identity.EffectiveTime, processedAccount.Identity.EffectiveTime);
        Assert.Equal(account.Identity.ExpirationTime, processedAccount.Identity.ExpirationTime);
    }

}
