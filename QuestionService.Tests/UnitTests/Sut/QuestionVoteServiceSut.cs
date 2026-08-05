using AutoMapper;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Mocks;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class QuestionVoteServiceSut
{
    private readonly IQuestionVoteService _questionVoteService;

    public readonly IBaseEventProducer EventProducer =
        BaseEventProducerFixture.GetBaseEventProducerConfiguration();

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly IUnitOfWork UnitOfWork = RepositoryMocks.GetMockUnitOfWork().Object;
    public readonly IEntityProvider<UserDto> UserProvider = EntityProviderMocks.GetMockUserProvider().Object;

    public readonly IBaseRepository<VoteType> VoteTypeRepository =
        RepositoryMocks.GetMockVoteTypeRepository().Object;

    public QuestionVoteServiceSut(IBaseRepository<VoteType>? voteTypeRepository = null)
    {
        if (voteTypeRepository != null)
            VoteTypeRepository = voteTypeRepository;

        _questionVoteService = new Application.Services.QuestionVoteService(UnitOfWork, VoteTypeRepository,
            UserProvider, Mapper, EventProducer);
    }

    public IQuestionVoteService GetService()
    {
        return _questionVoteService;
    }
}
