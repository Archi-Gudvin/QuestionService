using Microsoft.Extensions.Options;
using Serilog;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Interfaces;
using QuestionService.Cache.Repositories.Base;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;

namespace QuestionService.Cache.Repositories;

public class QuestionCacheRepository : IQuestionCacheRepository
{
    private readonly IBaseCacheRepository<Question, long> _repository;

    public QuestionCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        ILogger logger)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<Question, long>(
            cacheProvider,
            new CacheQuestionMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds,
            logger
        );
    }

    public Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<Question>>> fetch,
        CancellationToken cancellationToken = default)

    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<long> tagIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            tagIds,
            CacheKeyHelper.GetTagKey,
            CacheKeyHelper.GetTagQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserQuestionsKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    private sealed class CacheQuestionMapping : ICacheEntityMapping<Question, long>
    {
        public long GetId(Question entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetQuestionKey(id);
        }

        public string GetValue(Question entity)
        {
            return entity.Id.ToString();
        }

        public long ParseIdFromKey(string key)
        {
            return CacheKeyHelper.GetIdFromKey(key);
        }

        public long ParseIdFromValue(string value)
        {
            return long.Parse(value);
        }
    }
}