using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class GetVoteServiceTests
{
    [Fact]
    public async Task GetAllAsync_ExistingVotes_ReturnsSuccess()
    {
        //Arrange
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByDtosAsync_ExistingDtos_ReturnsSuccess()
    {
        //Arrange
        var dtos = new List<VoteDto>
        {
            new(1, 2),
            new(2, 3),
            new(0, 0),
        };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetByDtosAsync(dtos);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByDtosAsync_SingleNonExistentDto_ReturnsVoteNotFound()
    {
        //Arrange
        var dtos = new List<VoteDto>
        {
            new(0, 0)
        };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetByDtosAsync(dtos);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByDtosAsync_MultipleNonExistentDtos_ReturnsVotesNotFound()
    {
        //Arrange
        var dtos = new List<VoteDto>
        {
            new(0, 0),
            new(0, 0)
        };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetByDtosAsync(dtos);

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
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

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
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

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
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

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
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

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
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

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
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        // Act
        var result = await getVoteService.GetVoteTypesVotesAsync(voteTypeIds);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}