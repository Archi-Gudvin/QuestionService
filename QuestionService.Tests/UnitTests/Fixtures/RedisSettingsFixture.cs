using QuestionService.Cache.Settings;

namespace QuestionService.Tests.UnitTests.Fixtures;

internal static class RedisSettingsFixture
{
    public static RedisSettings GetRedisSettingsConfiguration()
    {
        return new RedisSettings
        {
            TimeToLiveInSeconds = 300
        };
    }
}