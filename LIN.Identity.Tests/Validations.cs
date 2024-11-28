using LIN.Cloud.Identity.Services.Formats;
using LIN.Types.Cloud.Identity.Enumerations;
using LIN.Types.Cloud.Identity.Models;

namespace LIN.Identity.Tests;

public class AccountTests
{
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
