using FluentValidation;
using QuestionService.Application.Services;
using QuestionService.Application.Validators;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Tests.Mocks;
using QuestionService.Tests.UnitTests.Fixtures;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Sut;

internal class ViewServiceSut
{
    private readonly IViewDatabaseService _viewDatabaseService;
    private readonly IViewService _viewService;

    public readonly IViewCacheSyncRepository CacheRepository =
        new ViewCacheSyncRepository(new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()));

    public readonly IBaseRepository<Question> QuestionRepository =
        RepositoryMocks.GetMockQuestionRepository().Object;

    public readonly IEntityProvider<UserDto> UserProvider = EntityProviderMocks.GetMockUserProvider().Object;

    public readonly IValidator<IValidatableView> Validator =
        ValidatorFixture<IValidatableView>.GetValidator(new ViewValidator());

    public readonly IBaseRepository<View> ViewRepository = RepositoryMocks.GetMockViewRepository().Object;

    public ViewServiceSut(IDatabase? redisDatabase = null)
    {
        if (redisDatabase != null) CacheRepository = new ViewCacheSyncRepository(new RedisCacheProvider(redisDatabase));

        var service = new ViewService(CacheRepository, QuestionRepository, ViewRepository, UserProvider, Validator);

        _viewService = service;
        _viewDatabaseService = service;
    }

    public IViewService GetService()
    {
        return _viewService;
    }

    public IViewDatabaseService GetDatabaseService()
    {
        return _viewDatabaseService;
    }
}
