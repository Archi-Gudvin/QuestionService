using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Mocks;

namespace QuestionService.Tests.UnitTests.Sut;

internal class GetTagServiceSut
{
    private readonly IGetTagService _getTagService;

    public readonly IBaseRepository<Question> QuestionRepository =
        RepositoryMocks.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Tag> TagRepository = RepositoryMocks.GetMockTagRepository().Object;


    public GetTagServiceSut()
    {
        _getTagService = new GetTagService(TagRepository, QuestionRepository);
    }

    public IGetTagService GetService()
    {
        return _getTagService;
    }
}
