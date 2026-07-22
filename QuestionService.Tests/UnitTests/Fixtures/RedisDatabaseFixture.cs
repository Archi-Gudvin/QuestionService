using Moq;
using QuestionService.Cache.Helpers;
using QuestionService.Tests.TestData;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Fixtures;

internal static class RedisDatabaseFixture
{
    public static IDatabase GetRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                var views = ViewCacheData.GetViews();

                return GetValuesByKey(key, views);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetFalseResponseRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase
            .Setup(x => x.KeyExpireAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CommandFlags>()
            ))
            .ReturnsAsync(false);

        return mockDatabase.Object;
    }

    public static IDatabase GetEmptySetValuesDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                var views = ViewCacheData.GetEmptyValuesViews();

                return GetValuesByKey(key, views);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetInvalidSetValuesDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                var views = ViewCacheData.GetInvalidValuesViews();

                return GetValuesByKey(key, views);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetInvalidSetKeysDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                var views = ViewCacheData.GetInvalidKeysViews();

                return GetValuesByKey(key, views);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetSpamDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                var views = ViewCacheData.GetSpamViews();

                return GetValuesByKey(key, views);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    private static RedisValue[] GetValuesByKey(RedisKey key, (RedisValue[] Keys, RedisValue[] Values) views)
    {
        if (key == CacheKeyHelper.GetViewQuestionsKey()) return views.Keys;
        if (key.ToString().StartsWith(CacheKeyHelper.GetViewQuestionKey(0)[..^1]))
            return views.Values;

        return [];
    }
}
