using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Tests.Mocks;
using QuestionService.Tests.UnitTests.Sut;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class QuestionVoteServiceTests
{
    [Fact]
    public async Task UpvoteAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpvoteAsync_ExistingDownvote_ReturnsSuccess()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 4;
        const long questionId = 3;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpvoteAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteAsync_OwnPost_ReturnsCannotVoteForOwnPost()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotVoteForOwnPost, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteAsync_EmptyVoteTypeRepository_ReturnsVoteTypeNotFound()
    {
        //Arrange
        var questionVoteService =
            new QuestionVoteServiceSut(RepositoryMocks.GetEmptyMockRepository<VoteType>().Object).GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteAsync_LowReputationInitiator_ReturnsTooLowReputation()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 3;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteAsync_ExistingUpvote_ReturnsVoteAlreadyGiven()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionVoteService.UpvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 4;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_ExistingUpvote_ReturnsSuccess()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_OwnPost_ReturnsCannotVoteForOwnPost()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotVoteForOwnPost, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_LowReputationInitiator_ReturnsTooLowReputation()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_EmptyVoteTypeRepository_ReturnsVoteTypeNotFound()
    {
        //Arrange
        var questionVoteService =
            new QuestionVoteServiceSut(RepositoryMocks.GetEmptyMockRepository<VoteType>().Object).GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteAsync_ExistingDownvote_ReturnsVoteAlreadyGiven()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 3;

        //Act
        var result = await questionVoteService.DownvoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RemoveVoteAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionVoteService.RemoveVoteAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task RemoveVoteAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.RemoveVoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RemoveVoteAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionVoteService.RemoveVoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RemoveVoteAsync_NonExistentVote_ReturnsVoteNotFound()
    {
        //Arrange
        var questionVoteService = new QuestionVoteServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionVoteService.RemoveVoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}