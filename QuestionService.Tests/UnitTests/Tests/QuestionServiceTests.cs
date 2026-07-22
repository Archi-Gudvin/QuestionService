using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;
using QuestionService.Tests.Mocks;
using QuestionService.Tests.UnitTests.Sut;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class QuestionServiceTests
{
    [Fact]
    public async Task AskQuestionAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET"]);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task AskQuestionAsync_EmptyTags_ReturnsInvalidTags()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", []);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTags, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AskQuestionAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 0;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET"]);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AskQuestionAsync_NonExistentTags_ReturnsTagsNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", ["WrongTag", "Java"]);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task EditQuestionAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task EditQuestionAsync_EmptyTitle_ReturnsInvalidTitle()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(1, string.Empty, "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTitle, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task EditQuestionAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 0;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task EditQuestionAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(0, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task EditQuestionAsync_NotOwnerInitiator_ReturnsOperationForbidden()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 2;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task EditQuestionAsync_NonExistentTags_ReturnsTagsNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", ["WrongTag", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteQuestionAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteQuestionAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteQuestionAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteQuestionAsync_NotOwnerInitiator_ReturnsOperationForbidden()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_ExistingDownvote_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 4;
        const long questionId = 3;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_OwnPost_ReturnsCannotVoteForOwnPost()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotVoteForOwnPost, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_EmptyVoteTypeRepository_ReturnsVoteTypeNotFound()
    {
        //Arrange
        var questionService =
            new QuestionServiceSut(RepositoryMocks.GetEmptyMockRepository<VoteType>().Object).GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_LowReputationInitiator_ReturnsTooLowReputation()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 3;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestionAsync_ExistingUpvote_ReturnsVoteAlreadyGiven()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 4;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_ExistingUpvote_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_OwnPost_ReturnsCannotVoteForOwnPost()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotVoteForOwnPost, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_LowReputationInitiator_ReturnsTooLowReputation()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_EmptyVoteTypeRepository_ReturnsVoteTypeNotFound()
    {
        //Arrange
        var questionService =
            new QuestionServiceSut(RepositoryMocks.GetEmptyMockRepository<VoteType>().Object).GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestionAsync_ExistingDownvote_ReturnsVoteAlreadyGiven()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 3;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RemoveQuestionVoteAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionService.RemoveQuestionVoteAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }


    [Fact]
    public async Task RemoveQuestionVoteAsync_NonExistentInitiator_ReturnsUserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionService.RemoveQuestionVoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RemoveQuestionVoteAsync_NonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionService.RemoveQuestionVoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RemoveQuestionVoteAsync_NonExistentVote_ReturnsVoteNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceSut().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionService.RemoveQuestionVoteAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}