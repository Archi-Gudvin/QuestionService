using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enums;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetVoteTypeService(IBaseRepository<VoteType> voteTypeRepository) : IGetVoteTypeService
{
    public Task<QueryableResult<VoteType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var voteTypes = voteTypeRepository.GetAll();

        return Task.FromResult(QueryableResult<VoteType>.Success(voteTypes));
    }

    public async Task<CollectionResult<VoteType>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var voteTypes = await voteTypeRepository.GetAll()
            .Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        if (voteTypes.Length == 0)
            return ids.Count() switch
            {
                <= 1 => CollectionResult<VoteType>.Failure(ErrorMessage.VoteTypeNotFound,
                    (int)ErrorCodes.VoteTypeNotFound),
                > 1 => CollectionResult<VoteType>.Failure(ErrorMessage.VoteTypesNotFound,
                    (int)ErrorCodes.VoteTypesNotFound)
            };

        return CollectionResult<VoteType>.Success(voteTypes);
    }
}