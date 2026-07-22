using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.View;
using QuestionService.Tests.UnitTests.Configurations;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class ViewServiceTests
{
    [Fact]
    public async Task IncrementViewsAsync_ValidIpAddress_ReturnsSuccess()
    {
        //Arrange
        var dto = new IncrementViewsDto(1, null, "0.0.0.0", "someFingerprint");
        var viewService = new ViewServiceFactory().GetService();

        //Act
        var result = await viewService.IncrementViewsAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task IncrementViewsAsync_InvalidIpAddress_ReturnsInvalidDataFormat()
    {
        //Arrange
        var dto = new IncrementViewsDto(1, null, "WrongIp", "someFingerprint");
        var viewService = new ViewServiceFactory().GetService();

        //Act
        var result = await viewService.IncrementViewsAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidDataFormat, result.ErrorMessage);
    }

    [Fact]
    public async Task SyncViewsToDatabaseAsync_ExistingViews_ReturnsSuccess()
    {
        //Arrange
        var viewService = new ViewServiceFactory().GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Data.SyncedViewsCount); // There are 7 new views in total
    }

    [Fact]
    public async Task SyncViewsToDatabaseAsync_EmptySetValues_ReturnsNoSyncedViews()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetEmptySetValuesDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedViewsCount);
    }

    [Fact]
    public async Task SyncViewsToDatabaseAsync_InvalidSetKeys_ReturnsNoSyncedViews()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetInvalidSetKeysDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedViewsCount);
    }

    [Fact]
    public async Task SyncViewsToDatabaseAsync_InvalidSetValues_ReturnsNoSyncedViews()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetInvalidSetValuesDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedViewsCount);
    }

    [Fact]
    public async Task SyncViewsToDatabaseAsync_SpamViews_ReturnsFilteredSyncedViews()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetSpamDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.SyncedViewsCount); // Total 2 new views, others filtered or already exist
    }
}