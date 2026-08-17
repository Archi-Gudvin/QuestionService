using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enums;
using QuestionService.Application.Extensions;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetTagService(IBaseRepository<Tag> tagRepository, IBaseRepository<Question> questionRepository)
    : IGetTagService
{
    public QueryableResult<Tag> GetAll()
    {
        var tags = tagRepository.GetAll().AsNoTracking();

        return QueryableResult<Tag>.Success(tags);
    }

    public async Task<CollectionResult<Tag>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var tags = await tagRepository.GetAll()
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        if (tags.Length == 0) return CollectionResult<Tag>.TagsNotFound(ids.Count);

        return CollectionResult<Tag>.Success(tags);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedTags = await questionRepository.GetAll()
            .AsNoTracking()
            .Where(x => questionIds.Contains(x.Id))
            .Include(x => x.Tags)
            .Select(x => new KeyValuePair<long, IEnumerable<Tag>>(x.Id, x.Tags))
            .ToArrayAsync(cancellationToken);

        if (groupedTags.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Failure(ErrorMessage.TagsNotFound,
                (int)ErrorCodes.TagsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(groupedTags);
    }
}