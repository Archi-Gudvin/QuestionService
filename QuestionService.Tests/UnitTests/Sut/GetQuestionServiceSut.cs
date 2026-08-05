using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Mocks;

namespace QuestionService.Tests.UnitTests.Sut;

internal class GetQuestionServiceSut
{
    private readonly IGetQuestionService _getQuestionService;

    public readonly IBaseRepository<Question> QuestionRepository =
        RepositoryMocks.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Tag> TagRepository = RepositoryMocks.GetMockTagRepository().Object;

    public GetQuestionServiceSut()
    {
        _getQuestionService = new GetQuestionService(QuestionRepository, TagRepository);
    }

    public IGetQuestionService GetService()
    {
        return _getQuestionService;
    }
}
