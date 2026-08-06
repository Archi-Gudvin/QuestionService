using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Tests.UnitTests.Sut;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class ValidatingQuestionServiceTests
{
    [Fact]
    public async Task AskQuestionAsync_EmptyTags_ReturnsInvalidTags()
    {
        //Arrange
        var sut = new ValidatingQuestionServiceSut();
        var questionService = sut.GetService();
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
    public async Task EditQuestionAsync_EmptyTitle_ReturnsInvalidTitle()
    {
        //Arrange
        var sut = new ValidatingQuestionServiceSut();
        var questionService = sut.GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(1, string.Empty, "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTitle, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}
