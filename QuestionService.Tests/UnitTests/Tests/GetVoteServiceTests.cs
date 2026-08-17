using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Tests.UnitTests.Sut;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class GetVoteServiceTests
{
    [Fact]
    public void GetAll_ExistingVotes_ReturnsSuccess()
    {
        //Arrange
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = getVoteService.GetAll();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByUserAndQuestionAsync_ExistingKeys_ReturnsSuccess()
    {
        //Arrange
        var keys = new List<VoteKey>
        {
            new(1, 2),
            new(2, 3),
            new(0, 0),
        };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = await getVoteService.GetByUserAndQuestionAsync(keys);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByUserAndQuestionAsync_SingleNonExistentKey_ReturnsVoteNotFound()
    {
        //Arrange
        var keys = new List<VoteKey>
        {
            new(0, 0)
        };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = await getVoteService.GetByUserAndQuestionAsync(keys);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByUserAndQuestionAsync_MultipleNonExistentKeys_ReturnsVotesNotFound()
    {
        //Arrange
        var keys = new List<VoteKey>
        {
            new(0, 0),
            new(0, 0)
        };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = await getVoteService.GetByUserAndQuestionAsync(keys);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetQuestionsVotesAsync_ExistingQuestionIds_ReturnsSuccess()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = await getVoteService.GetQuestionsVotesAsync(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetQuestionsVotesAsync_NonExistentQuestionId_ReturnsVotesNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = await getVoteService.GetQuestionsVotesAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetUsersVotesAsync_ExistingUserIds_ReturnsSuccess()
    {
        //Arrange
        var userIds = new List<long> { 1, 2, 0 };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = await getVoteService.GetUsersVotesAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetUsersVotesAsync_NonExistentUserId_ReturnsVotesNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0 };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        //Act
        var result = await getVoteService.GetUsersVotesAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetVoteTypesVotesAsync_ExistingVoteTypeIds_ReturnsSuccess()
    {
        // Arrange
        var voteTypeIds = new List<long> { 1, 2, 0 };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        // Act
        var result = await getVoteService.GetVoteTypesVotesAsync(voteTypeIds);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetVoteTypesVotesAsync_NonExistentVoteTypeId_ReturnsVotesNotFound()
    {
        // Arrange
        var voteTypeIds = new List<long> { 0 };
        var getVoteService = new CacheGetVoteServiceSut().GetService();

        // Act
        var result = await getVoteService.GetVoteTypesVotesAsync(voteTypeIds);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}