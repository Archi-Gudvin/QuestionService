using QuestionService.Application.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GetViewServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllAsync_ExistingViews_ReturnsSuccess()
    {
        //Arrange
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIdsAsync_ExistingIds_ReturnsSuccess()
    {
        //Arrange
        var viewIds = new List<long> { 1, 2, 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetByIdsAsync(viewIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIdsAsync_SingleNonExistentId_ReturnsViewNotFound()
    {
        //Arrange
        var viewIds = new List<long> { 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetByIdsAsync(viewIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIdsAsync_MultipleNonExistentIds_ReturnsViewsNotFound()
    {
        //Arrange
        var viewIds = new List<long> { 0, 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetByIdsAsync(viewIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersViewsAsync_ExistingUserIds_ReturnsSuccess()
    {
        //Arrange
        var userIds = new List<long> { 1, 2, 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetUsersViewsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersViewsAsync_NonExistentUserId_ReturnsViewsNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetUsersViewsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsViewsAsync_ExistingQuestionIds_ReturnsSuccess()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetQuestionsViewsAsync(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsViewsAsync_NonExistentQuestionId_ReturnsViewsNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetQuestionsViewsAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}