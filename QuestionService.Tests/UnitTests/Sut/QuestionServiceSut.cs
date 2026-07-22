using AutoMapper;
using FluentValidation;
using QuestionService.Application.Validators;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Tests.Mocks;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class QuestionServiceSut
{
    private readonly IQuestionService _questionService;

    public readonly IBaseEventProducer EventProducer =
        BaseEventProducerFixture.GetBaseEventProducerConfiguration();

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = RepositoryMocks.GetMockTagRepository().Object;

    public readonly IUnitOfWork UnitOfWork = RepositoryMocks.GetMockUnitOfWork().Object;
    public readonly IEntityProvider<UserDto> UserProvider = EntityProviderMocks.GetMockUserProvider().Object;

    public readonly IValidator<IValidatableQuestion> Validator =
        ValidatorFixture<IValidatableQuestion>.GetValidator(new QuestionValidator());

    public readonly IBaseRepository<VoteType> VoteTypeRepository =
        RepositoryMocks.GetMockVoteTypeRepository().Object;

    public QuestionServiceSut(IBaseRepository<VoteType>? voteTypeRepository = null)
    {
        if (voteTypeRepository != null)
            VoteTypeRepository = voteTypeRepository;

        _questionService = new Application.Services.QuestionService(UnitOfWork, TagRepository, VoteTypeRepository,
            UserProvider, Mapper, EventProducer, Validator);
    }

    public IQuestionService GetService()
    {
        return _questionService;
    }
}
