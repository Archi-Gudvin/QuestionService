using StackExchange.Redis;

namespace QuestionService.Tests.TestData;

internal static class ViewCacheData
{
    public static (RedisValue[] Keys, RedisValue[] Values) GetViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3")
            ],
            [
                new RedisValue("1"), new RedisValue("2"), new RedisValue("0.0.0.0,testFingerprint")
            ]
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetEmptyValuesViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3")
            ], []
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetInvalidValuesViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3")
            ],
            [
                new RedisValue("WrongFormat"), new RedisValue("0")
            ]
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetInvalidKeysViews()
    {
        return (
            [
                new RedisValue("view:question:wrongFormat")
            ],
            [
                new RedisValue("1"), new RedisValue("2"), new RedisValue("0.0.0.0,testFingerprint")
            ]
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetSpamViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3"), new RedisValue("view:question:4")
            ],
            [
                new RedisValue("1"), new RedisValue("0.0.0.0,testFingerprint1"),
                new RedisValue("0.0.0.0,testFingerprint2"), new RedisValue("0.0.0.0,testFingerprint3"),
                new RedisValue("0.0.0.0,testFingerprint4"), new RedisValue("0.0.0.0,testFingerprint5"),
                new RedisValue("0.0.0.0,testFingerprint6"), new RedisValue("0.0.0.0,testFingerprint7"),
                new RedisValue("0.0.0.0,testFingerprint8")
            ]
        );
    }
}
