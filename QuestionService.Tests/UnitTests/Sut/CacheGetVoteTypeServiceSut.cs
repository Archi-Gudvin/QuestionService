using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

public class CacheGetVoteTypeServiceSut
{
    private readonly IGetVoteTypeService _getVoteTypeService;

    public readonly GetVoteTypeService InnerGetVoteTypeService =
        (GetVoteTypeService)new GetVoteTypeServiceSut().GetService();

    public readonly IVoteTypeCacheRepository VoteCacheRepository =
        new VoteTypeCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetVoteTypeServiceSut()
    {
        _getVoteTypeService = new CacheGetVoteTypeService(VoteCacheRepository, InnerGetVoteTypeService);
    }

    public IGetVoteTypeService GetService()
    {
        return _getVoteTypeService;
    }
}
