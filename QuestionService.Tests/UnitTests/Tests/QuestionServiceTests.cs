using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Question;
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
}