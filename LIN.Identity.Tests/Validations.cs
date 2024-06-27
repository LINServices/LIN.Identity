using LIN.Types.Cloud.Identity.Models;

namespace LIN.Identity.Tests;


public class Validations
{


    /// <summary>
    /// Validar cuentas.
    /// </summary>
    [Fact]
    public void ValidateAccounts()
    {

        var accounts = new Dictionary<AccountModel, bool>
        {
            {
                new()
                    {
                        IdentityService = Types.Cloud.Identity.Enumerations.IdentityService.LIN,
                        Identity = new()
                        {
                        },
                        Name = "John Doe",
                        Password = "password",
                        Visibility = Types.Cloud.Identity.Enumerations.Visibility.Visible
                    }, false
            },
             {
                new()
                    {
                        IdentityService = Types.Cloud.Identity.Enumerations.IdentityService.LIN,
                        Identity = new()
                        {
                            Unique = "johnDoe"
                        },
                        Name = "John Doe",
                        Password = "password",
                        Visibility = Types.Cloud.Identity.Enumerations.Visibility.Visible
                    }, true
            },
              {
                new()
                    {

                    }, false
            },

             {
                new()
                    {
                        IdentityService = Types.Cloud.Identity.Enumerations.IdentityService.LIN,
                        Identity = new()
                        {
                            Unique = "johnDoe"
                        },
                        Name = "",
                        Password = "",
                        Visibility = Types.Cloud.Identity.Enumerations.Visibility.Visible
                    }, false
            },
        };


        foreach (var account in accounts)
        {
            (bool final, _) = LIN.Cloud.Identity.Services.Formats.Account.Validate(account.Key);
            Assert.Equal(account.Value, final);
        }

    }



}