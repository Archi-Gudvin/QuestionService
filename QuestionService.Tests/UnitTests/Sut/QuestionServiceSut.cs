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

internal class QuestionServiceSut
{
    private readonly IQuestionService _questionService;

    public readonly IBaseEventProducer EventProducer =
        BaseEventProducerFixture.GetBaseEventProducerConfiguration();

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = RepositoryMocks.GetMockTagRepository().Object;

    public readonly IUnitOfWork UnitOfWork = RepositoryMocks.GetMockUnitOfWork().Object;
    public readonly IEntityProvider<UserDto> UserProvider = EntityProviderMocks.GetMockUserProvider().Object;

    public QuestionServiceSut()
    {
        _questionService =
            new Application.Services.QuestionService(UnitOfWork, TagRepository, UserProvider, Mapper, EventProducer);
    }

    public IQuestionService GetService()
    {
        return _questionService;
    }
}