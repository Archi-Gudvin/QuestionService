using Moq;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Tests.UnitTests.Fixtures;

internal static class QuestionServiceFixture
{
    public static IQuestionService GetQuestionServiceConfiguration()
    {
        var mock = new Mock<IQuestionService>();
        var result = BaseResult<QuestionDto>.Success(new QuestionDto(1, "Title", "Body", [], 1));

        mock.Setup(x => x.AskQuestionAsync(It.IsAny<long>(), It.IsAny<AskQuestionDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        mock.Setup(x => x.EditQuestionAsync(It.IsAny<long>(), It.IsAny<EditQuestionDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        mock.Setup(x => x.DeleteQuestionAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock.Object;
    }
}