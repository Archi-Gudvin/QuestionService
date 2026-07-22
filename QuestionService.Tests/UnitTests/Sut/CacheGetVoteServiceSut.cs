using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class CacheGetVoteServiceSut
{
    private readonly IGetVoteService _cacheGetVoteService;

    public readonly GetVoteService InnerGetVoteService =
        (GetVoteService)new GetVoteServiceSut().GetService();

    public readonly IVoteCacheRepository VoteCacheRepository =
        new VoteCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetVoteServiceSut()
    {
        _cacheGetVoteService = new CacheGetVoteService(VoteCacheRepository, InnerGetVoteService);
    }

    public IGetVoteService GetService()
    {
        return _cacheGetVoteService;
    }
}
