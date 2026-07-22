using QuestionService.Application.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class GetVoteTypeServiceTests
{
    [Fact]
    public async Task GetAllAsync_ExistingVoteTypes_ReturnsSuccess()
    {
        //Arrange
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        //Act
        var result = await getVoteTypeService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_ExistingIds_ReturnsSuccess()
    {
        // Arrange
        var ids = new List<long> { 1, 2, 0 };
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        // Act
        var result = await getVoteTypeService.GetByIdsAsync(ids);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_SingleNonExistentId_ReturnsVoteTypeNotFound()
    {
        // Arrange
        var ids = new List<long> { 0 };
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        // Act
        var result = await getVoteTypeService.GetByIdsAsync(ids);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_MultipleNonExistentIds_ReturnsVoteTypesNotFound()
    {
        // Arrange
        var ids = new List<long> { 0, 0 };
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        // Act
        var result = await getVoteTypeService.GetByIdsAsync(ids);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}