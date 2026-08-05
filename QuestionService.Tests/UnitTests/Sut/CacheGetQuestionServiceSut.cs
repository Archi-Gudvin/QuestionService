using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class CacheGetQuestionServiceSut
{
    private readonly IGetQuestionService _cacheGetQuestionService;

    public readonly GetQuestionService InnerGetQuestionService =
        (GetQuestionService)new GetQuestionServiceSut().GetService();

    public readonly IQuestionCacheRepository QuestionCacheRepository =
        new QuestionCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()),
            new Mock<ILogger>().Object);

    public CacheGetQuestionServiceSut()
    {
        _cacheGetQuestionService = new CacheGetQuestionService(QuestionCacheRepository, InnerGetQuestionService);
    }

    public IGetQuestionService GetService()
    {
        return _cacheGetQuestionService;
    }
}
