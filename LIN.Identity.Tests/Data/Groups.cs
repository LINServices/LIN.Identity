using LIN.Cloud.Identity.Data;
using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Types.Cloud.Identity.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Identity.Tests.Data;

public class GroupsTests
{
    private static DataContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        return new DataContext(options);
    }

    [Fact]
    public async Task ReadByIdentity_ShouldReturnSuccessResponse_WhenGroupExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var groups = new Groups(context);
        var groupModel = new GroupModel
        {
            Name = "Test Group",
            Identity = new IdentityModel { Unique = "unique", Id = 0 }
        };

        context.Groups.Add(groupModel);
        await context.SaveChangesAsync();

        // Act
        var response = await groups.ReadByIdentity(1);

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotNull(response.Model);
        Assert.Equal("Test Group", response.Model.Name);
    }

    [Fact]
    public async Task ReadByIdentity_ShouldReturnNotRowsResponse_WhenGroupDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var groups = new Groups(context);

        // Act
        var response = await groups.ReadByIdentity(999);

        // Assert
        Assert.Equal(Responses.NotRows, response.Response);
    }

    [Fact]
    public async Task ReadAll_ShouldReturnSuccessResponse_WhenGroupsExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var groups = new Groups(context);
        var groupModel = new GroupModel
        {
            OwnerId = 1,
            Name = "Test Group",
            Identity = new()
            {
                Id = 1,
                Unique = "unique"
            }
        };

        context.Groups.Add(groupModel);
        await context.SaveChangesAsync();

        // Act
        var response = await groups.ReadAll(1);

        // Assert
        Assert.Equal(Responses.Success, response.Response);
        Assert.NotEmpty(response.Models);
    }

    [Fact]
    public async Task ReadAll_ShouldReturnNotRowsResponse_WhenNoGroupsExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var groups = new Groups(context);

        // Act
        var response = await groups.ReadAll(999);

        // Assert
        Assert.Empty(response.Models);
    }

}