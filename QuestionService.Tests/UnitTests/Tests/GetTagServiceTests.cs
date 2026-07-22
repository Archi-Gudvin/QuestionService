using QuestionService.Application.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class GetTagServiceTests
{
    [Fact]
    public async Task GetAllAsync_ExistingTags_ReturnsSuccess()
    {
        //Arrange
        var getTagService = new CacheGetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_ExistingIds_ReturnsSuccess()
    {
        //Arrange
        var tagIds = new List<long> { 1, 2, 0 };
        var getTagService = new CacheGetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByIdsAsync(tagIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_SingleNonExistentId_ReturnsTagNotFound()
    {
        //Arrange
        var tagIds = new List<long> { 0 };
        var getTagService = new CacheGetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByIdsAsync(tagIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_MultipleNonExistentIds_ReturnsTagsNotFound()
    {
        //Arrange
        var tagIds = new List<long> { 0, 0 };
        var getTagService = new CacheGetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByIdsAsync(tagIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetQuestionsTagsAsync_ExistingQuestionIds_ReturnsSuccess()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getTagService = new CacheGetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetQuestionsTagsAsync(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetQuestionsTagsAsync_NonExistentQuestionIds_ReturnsTagsNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getTagService = new CacheGetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetQuestionsTagsAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}