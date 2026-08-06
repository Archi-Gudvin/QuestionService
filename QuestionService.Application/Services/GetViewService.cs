using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enums;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetViewService(IBaseRepository<View> viewRepository) : IGetViewService
{
    public Task<QueryableResult<View>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var views = viewRepository.GetAll();

        return Task.FromResult(QueryableResult<View>.Success(views));
    }


    public async Task<CollectionResult<View>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var views = await viewRepository.GetAll().Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken);

        if (views.Length == 0)
            return ids.Count switch
            {
                <= 1 => CollectionResult<View>.Failure(ErrorMessage.ViewNotFound, (int)ErrorCodes.ViewNotFound),
                > 1 => CollectionResult<View>.Failure(ErrorMessage.ViewsNotFound, (int)ErrorCodes.ViewsNotFound),
            };

        return CollectionResult<View>.Success(views);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(
        IReadOnlyCollection<long> userIds, CancellationToken cancellationToken = default)
    {
        var views = (await viewRepository.GetAll()
                .Where(x => x.UserId.HasValue && userIds.Contains(x.UserId.Value))
                .GroupBy(x => x.UserId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<View>>(x.Key!.Value, x.ToArray()))
            .ToArray();


        if (views.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(views);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default)
    {
        var views = (await viewRepository.GetAll()
                .Where(x => questionIds.Contains(x.QuestionId))
                .GroupBy(x => x.QuestionId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<View>>(x.Key, x.ToArray()))
            .ToArray();

        if (views.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(views);
    }
}