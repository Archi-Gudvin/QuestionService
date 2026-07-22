using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Mocks;

namespace QuestionService.Tests.UnitTests.Sut;

public class GetVoteTypeServiceSut
{
    private readonly IGetVoteTypeService _getVoteTypeService;

    public IBaseRepository<VoteType> VoteTypeRepository = RepositoryMocks.GetMockVoteTypeRepository().Object;

    public GetVoteTypeServiceSut()
    {
        _getVoteTypeService = new GetVoteTypeService(VoteTypeRepository);
    }

    public IGetVoteTypeService GetService()
    {
        return _getVoteTypeService;
    }
}
