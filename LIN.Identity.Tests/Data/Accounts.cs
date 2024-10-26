using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Cloud.Identity.Persistence.Models;
using LIN.Cloud.Identity.Services.Models;
using LIN.Types.Cloud.Identity.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Identity.Tests.Data;

public class Accounts
{

    /// <summary>
    /// Obtener base de datos en memoria.
    /// </summary>
    /// <returns></returns>
    private static DataContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        return new DataContext(options);
    }

    [Fact]
    public async Task Create_ShouldReturnSuccessResponse_WhenAccountIsCreated()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);
        var accountModel = new AccountModel { Name = "Test Account" };

        // Act
        var response = await accounts.Create(accountModel);

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotNull(response.Model);
        Assert.Equal("Test Account", response.Model.Name);
    }

    [Fact]
    public async Task Create_ShouldReturnExistAccountResponse_WhenOrganizationNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);
        var accountModel = new AccountModel { Name = "Test Account" };

        // Act
        var response = await accounts.Create(accountModel, organization: 999);

        // Assert
        Assert.Equal(Responses.ExistAccount, response.Response);
    }

    [Fact]
    public async Task Read_ShouldReturnNotRowsResponse_WhenAccountDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);

        // Act
        var response = await accounts.Read(999, new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.NotRows, response.Response);
    }

    [Fact]
    public async Task ReadByUnique_ShouldReturnSuccessResponse_WhenAccountExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);
        var accountModel = new AccountModel
        {
            Name = "Test Account",
            Identity = new IdentityModel
            {
                Unique = "unique-id",
                EffectiveTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddDays(1)
            }
        };
        await context.Accounts.AddAsync(accountModel);
        await context.SaveChangesAsync();

        // Act
        var response = await accounts.Read("unique-id", new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotNull(response.Model);
        Assert.Equal("Test Account", response.Model.Name);
    }

    [Fact]
    public async Task ReadByUnique_ShouldReturnNotRowsResponse_WhenAccountDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);

        // Act
        var response = await accounts.Read("non-existent-id", new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.NotRows, response.Response);
    }

    [Fact]
    public async Task ReadByIdentity_ShouldReturnSuccessResponse_WhenAccountExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);
        var accountModel = new AccountModel
        {
            Name = "Test Account",
            Identity = new IdentityModel
            {
                Unique = "unique-id",
                EffectiveTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddDays(1)
            }
        };
        await context.Accounts.AddAsync(accountModel);
        await context.SaveChangesAsync();

        // Act
        var response = await accounts.ReadByIdentity(1, new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotNull(response.Model);
        Assert.Equal("Test Account", response.Model.Name);
    }

    [Fact]
    public async Task ReadByIdentity_ShouldReturnNotRowsResponse_WhenAccountDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);

        // Act
        var response = await accounts.ReadByIdentity(999, new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.NotRows, response.Response);
    }

    [Fact]
    public async Task Search_ShouldReturnSuccessResponse_WhenAccountsMatchPattern()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);
        var accountModel = new AccountModel
        {
            Name = "Test Account",
            Identity = new IdentityModel
            {
                Unique = "Test",
                EffectiveTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddDays(1)
            }
        };
        await context.Accounts.AddAsync(accountModel);
        await context.SaveChangesAsync();

        // Act
        var response = await accounts.Search("Test", new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotEmpty(response.Models);
    }

    [Fact]
    public async Task Search_ShouldReturnNotRowsResponse_WhenNoAccountsMatchPattern()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);

        // Act
        var response = await accounts.Search("NonExistentPattern", new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.NotRows, response.Response);
    }

    [Fact]
    public async Task FindAll_ShouldReturnSuccessResponse_WhenAccountsExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);
        var accountModel = new AccountModel
        {
            Name = "Test Account",
            Identity = new()
            {
                Unique = "test",
                EffectiveTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddDays(1)
            }
        };
        await context.Accounts.AddAsync(accountModel);
        await context.SaveChangesAsync();

        // Act
        var response = await accounts.FindAll([accountModel.Id], new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotEmpty(response.Models);
    }

    [Fact]
    public async Task FindAll_ShouldReturnNotRowsResponse_WhenAccountsDoNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);

        // Act
        var response = await accounts.FindAll([999], new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.NotRows, response.Response);
    }

    [Fact]
    public async Task FindAllByIdentities_ShouldReturnSuccessResponse_WhenAccountsExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);
        var accountModel = new AccountModel
        {
            Name = "Test Account",
            Identity = new()
            {
                Unique = "test",
                EffectiveTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddDays(1)
            }
        };
        await context.Accounts.AddAsync(accountModel);
        await context.SaveChangesAsync();

        // Act
        var response = await accounts.FindAllByIdentities([1], new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotEmpty(response.Models);
    }

    [Fact]
    public async Task FindAllByIdentities_ShouldReturnNotRowsResponse_WhenAccountsDoNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var accounts = new Cloud.Identity.Data.Accounts(context);

        // Act
        var response = await accounts.FindAllByIdentities([999], new QueryObjectFilter());

        // Assert
        Assert.Equal(Responses.NotRows, response.Response);
    }

}