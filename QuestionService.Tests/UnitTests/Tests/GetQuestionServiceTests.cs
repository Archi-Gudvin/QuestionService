using QuestionService.Application.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class GetQuestionServiceTests
{
    [Fact]
    public async Task GetAllAsync_ExistingQuestions_ReturnsSuccess()
    {
        //Arrange
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }


    [Fact]
    public async Task GetByIdsAsync_ExistingIds_ReturnsSuccess()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdsAsync(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_SingleNonExistentId_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdsAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByIdsAsync_MultipleNonExistentIds_ReturnsQuestionsNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0, 0 };
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdsAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetQuestionsWithTagsAsync_ExistingTagIds_ReturnsSuccess()
    {
        //Arrange
        var tagIds = new List<long> { 1, 2, 0 };
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetQuestionsWithTagsAsync(tagIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetQuestionsWithTagsAsync_NonExistentTagIds_ReturnsQuestionsNotFound()
    {
        //Arrange
        var tagIds = new List<long> { 0, 0 };
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetQuestionsWithTagsAsync(tagIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetUsersQuestionsAsync_ExistingUserIds_ReturnsSuccess()
    {
        //Arrange
        var userIds = new List<long> { 1, 2, 0 };
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetUsersQuestionsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetUsersQuestionsAsync_NonExistentUserIds_ReturnsQuestionsNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0, 0 };
        var getQuestionService = new CacheGetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetUsersQuestionsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}