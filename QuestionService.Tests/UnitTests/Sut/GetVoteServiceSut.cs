using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Mocks;

namespace QuestionService.Tests.UnitTests.Sut;

internal class GetVoteServiceSut
{
    private readonly IGetVoteService _getVoteService;

    public readonly IBaseRepository<Vote> VoteRepository = RepositoryMocks.GetMockVoteRepository().Object;

    public GetVoteServiceSut()
    {
        _getVoteService = new GetVoteService(VoteRepository);
    }

    public IGetVoteService GetService()
    {
        return _getVoteService;
    }
}
