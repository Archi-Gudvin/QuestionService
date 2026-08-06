using Microsoft.Extensions.Options;
using Serilog;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Interfaces;
using QuestionService.Cache.Repositories.Base;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;

namespace QuestionService.Cache.Repositories;

public class ViewCacheRepository : IViewCacheRepository
{
    private readonly IBaseCacheRepository<View, long> _repository;

    public ViewCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        ILogger logger)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<View, long>(
            cacheProvider,
            new CacheViewMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds,
            logger
        );
    }

    public Task<IEnumerable<View>> GetByIdsAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<View>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserViewsKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserViewsKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IEnumerable<long> questionIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionKey,
            CacheKeyHelper.GetQuestionViewsKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    private sealed class CacheViewMapping : ICacheEntityMapping<View, long>
    {
        public long GetId(View entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetViewKey(id);
        }

        public string GetValue(View entity)
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