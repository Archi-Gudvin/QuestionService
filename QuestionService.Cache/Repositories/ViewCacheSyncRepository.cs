using System.Net;
using QuestionService.Application.Resources;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Interfaces;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Repository.Cache;

namespace QuestionService.Cache.Repositories;

public class ViewCacheSyncRepository(ICacheProvider cache) : IViewCacheSyncRepository
{
    private const char ViewKeySeparator = ',';

    public async Task<IEnumerable<View>> GetValidViewsAsync(CancellationToken cancellationToken = default)
    {
        // Removing invalid keys
        var allViewKeys = await cache.SetStringMembersAsync(CacheKeyHelper.GetViewQuestionsKey(), cancellationToken);
        var validViewKeys = allViewKeys.Where(IsValidKey);

        // Removing invalid values
        var allViewValues = await cache.SetsStringMembersAsync(validViewKeys, cancellationToken);
        var validViewValues = allViewValues.Select(x =>
            new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Value.Where(IsValidValue)));

        var views = validViewValues.SelectManyFromGroupedValues(ViewParsingHelpers.ParseQuestionIdFromKey,
            ViewParsingHelpers.ParseViewFromValue);

        return views;
    }

    public async Task DeleteAllViewsAsync(CancellationToken cancellationToken = default)
    {
        var allViewKeys = await cache.SetStringMembersAsync(CacheKeyHelper.GetViewQuestionsKey(), cancellationToken);
        var keysToDelete = allViewKeys.Prepend(CacheKeyHelper.GetViewQuestionsKey());
        await cache.KeysDeleteAsync(keysToDelete, cancellationToken: cancellationToken);
    }

    public Task AddViewAsync(IncrementViewsDto dto, CancellationToken cancellationToken = default)
    {
        var key = CacheKeyHelper.GetViewQuestionKey(dto.QuestionId);
        var value = GetViewValue(dto);

        var keyValueMap = new List<KeyValuePair<string, IEnumerable<string>>>
        {
            new(key, [value]),
            new(CacheKeyHelper.GetViewQuestionsKey(), [key])
        };

        return cache.SetsAddAsync(keyValueMap, cancellationToken: cancellationToken);
    }

    private static string GetViewValue(IncrementViewsDto dto)
    {
        if (dto.UserId != null)
            return dto.UserId.ToString()!;

        ArgumentException.ThrowIfNullOrWhiteSpace(dto.UserIp);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.UserFingerprint);
        var ip = IPAddress.Parse(dto.UserIp); //Throws an exception if dto.UserIp is not IP

        return $"{ip.ToString()}{ViewKeySeparator}{dto.UserFingerprint}";
    }

    private static bool IsValidKey(string key)
    {
        try
        {
            CacheKeyHelper.GetIdFromKey(key);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsValidValue(string value)
    {
        return long.TryParse(value, out _)
               || value.Split(ViewKeySeparator).Length == 2;
    }

    private static class ViewParsingHelpers
    {
        public static long ParseQuestionIdFromKey(string key)
        {
            if (!IsValidKey(key))
                throw new FormatException(ErrorMessage.InvalidCacheDataFormat);

            return CacheKeyHelper.GetIdFromKey(key);
        }

        public static View ParseViewFromValue(long questionId, string value)
        {
            var view = new View
            {
                QuestionId = questionId
            };

            if (!long.TryParse(value, out var userId))
            {
                var parts = GetValueParts(value);

                view.UserIp = parts[0];
                view.UserFingerprint = parts[1];
            }
            else
            {
                view.UserId = userId;
            }

            return view;
        }

        private static string[] GetValueParts(string s)
        {
            var parts = s.Split(ViewKeySeparator);

            if (parts.Length != 2
                || StringHelper.AnyNullOrWhiteSpace(parts)
                || !IPAddress.TryParse(parts[0], out _))
                throw new FormatException(ErrorMessage.InvalidCacheDataFormat);

            return parts;
        }
    }
}