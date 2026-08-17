using Microsoft.Extensions.Options;
using Serilog;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Interfaces;
using QuestionService.Cache.Repositories.Base;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;

namespace QuestionService.Cache.Repositories;

public class VoteCacheRepository : IVoteCacheRepository
{
    private const string VoteValuePattern = "{0},{1}";

    private readonly IBaseCacheRepository<Vote, VoteKey> _repository;

    public VoteCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        ILogger logger)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<Vote, VoteKey>(
            cacheProvider,
            new VoteCacheMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds,
            logger
        );
    }

    public Task<IEnumerable<Vote>> GetByUserAndQuestionAsync(IEnumerable<VoteKey> keys,
        Func<IEnumerable<VoteKey>, CancellationToken, Task<IEnumerable<Vote>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(keys, fetch, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IEnumerable<long> questionIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionKey,
            CacheKeyHelper.GetQuestionVotesKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserVotesKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserVotesKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetVoteTypesVotesAsync(
        IEnumerable<long> voteTypeIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            voteTypeIds,
            CacheKeyHelper.GetVoteTypeKey,
            CacheKeyHelper.GetVoteTypeVotesKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    private sealed class VoteCacheMapping : ICacheEntityMapping<Vote, VoteKey>
    {
        public VoteKey GetId(Vote entity)
        {
            return new VoteKey(entity.QuestionId, entity.UserId);
        }

        public string GetKey(VoteKey id)
        {
            return CacheKeyHelper.GetVoteKey(id.QuestionId, id.UserId);
        }

        public string GetValue(Vote entity)
        {
            return string.Format(VoteValuePattern, entity.QuestionId, entity.UserId);
        }

        public VoteKey ParseIdFromKey(string key)
        {
            var ex = new ArgumentException($"Invalid key format: {key}");

            var parts = key.Split(':');
            if (parts.Length < 2)
                throw ex;

            parts = parts[1].Split(',');
            if (parts.Length < 2)
                throw ex;

            var ids = parts.Select(long.Parse).ToArray();
            return new VoteKey(ids[0], ids[1]);
        }

        public VoteKey ParseIdFromValue(string value)
        {
            var parts = value.Split(',');
            if (parts.Length < 2)
                throw new ArgumentException($"Invalid value format: {value}");

            var ids = parts.Select(long.Parse).ToArray();

            return new VoteKey(ids[0], ids[1]);
        }
    }
}