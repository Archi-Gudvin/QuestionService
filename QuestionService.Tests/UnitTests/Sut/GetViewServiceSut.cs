using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Mocks;
using View = QuestionService.Domain.Entities.View;

namespace QuestionService.Tests.UnitTests.Sut;

internal class GetViewServiceSut
{
    private readonly IGetViewService _getViewService;

    public readonly IBaseRepository<Question> QuestionRepository =
        RepositoryMocks.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<View> ViewRepository = RepositoryMocks.GetMockViewRepository().Object;

    public GetViewServiceSut()
    {
        _getViewService = new GetViewService(ViewRepository);
    }

    public IGetViewService GetService()
    {
        return _getViewService;
    }
}
