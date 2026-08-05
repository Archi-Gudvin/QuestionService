using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class CacheGetViewServiceSut
{
    private readonly IGetViewService _cacheGetViewService;

    public readonly GetViewService InnerGetViewService =
        (GetViewService)new GetViewServiceSut().GetService();

    public readonly IViewCacheRepository ViewCacheRepository =
        new ViewCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetViewServiceSut()
    {
        _cacheGetViewService = new CacheGetViewService(ViewCacheRepository, InnerGetViewService);
    }

    public IGetViewService GetService()
    {
        return _cacheGetViewService;
    }
}
