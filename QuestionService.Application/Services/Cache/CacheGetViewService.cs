using QuestionService.Application.Enums;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetViewService(IViewCacheRepository cacheRepository, IGetViewService inner) : IGetViewService
{
    public Task<QueryableResult<View>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<View>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var views = (await cacheRepository.GetByIdsAsync(idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (views.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<View>.Failure(ErrorMessage.ViewNotFound, (int)ErrorCodes.ViewNotFound),
                > 1 => CollectionResult<View>.Failure(ErrorMessage.ViewsNotFound, (int)ErrorCodes.ViewsNotFound)
            };

        return CollectionResult<View>.Success(views);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedViews = (await cacheRepository.GetUsersViewsAsync(userIds,
            async (idsToFetch, ct) => (await inner.GetUsersViewsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedViews.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(groupedViews);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedViews = (await cacheRepository.GetQuestionsViewsAsync(questionIds,
            async (idsToFetch, ct) => (await inner.GetQuestionsViewsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedViews.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(groupedViews);
    }
}