using QuestionService.Application.Enums;
using QuestionService.Application.Extensions;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetTagService(ITagCacheRepository cacheRepository, IGetTagService inner) : IGetTagService
{
    public QueryableResult<Tag> GetAll()
    {
        return inner.GetAll();
    }

    public async Task<CollectionResult<Tag>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var tags = (await cacheRepository.GetByIdsAsync(ids,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (tags.Length == 0) return CollectionResult<Tag>.TagsNotFound(ids.Count);

        return CollectionResult<Tag>.Success(tags);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedTags = (await cacheRepository.GetQuestionsTagsAsync(questionIds,
            async (idsToFetch, ct) => (await inner.GetQuestionsTagsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedTags.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Failure(ErrorMessage.TagsNotFound,
                (int)ErrorCodes.TagsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(groupedTags);
    }
}