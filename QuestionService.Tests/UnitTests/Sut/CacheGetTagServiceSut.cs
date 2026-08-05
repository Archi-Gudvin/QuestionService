using Microsoft.Extensions.Options;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class CacheGetTagServiceSut
{
    private readonly IGetTagService _cacheGetTagService;

    public readonly IGetTagService InnerGetTagService =
        new GetTagServiceSut().GetService();

    public readonly ITagCacheRepository TagCacheRepository =
        new TagCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetTagServiceSut()
    {
        _cacheGetTagService = new CacheGetTagService(TagCacheRepository, InnerGetTagService);
    }

    public IGetTagService GetService()
    {
        return _cacheGetTagService;
    }
}
