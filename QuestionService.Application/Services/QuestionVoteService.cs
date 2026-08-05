using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enum;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class QuestionVoteService(
    IUnitOfWork unitOfWork,
    IBaseRepository<VoteType> voteTypeRepository,
    IEntityProvider<UserDto> userProvider,
    IMapper mapper,
    IBaseEventProducer producer)
    : IQuestionVoteService
{
    public Task<BaseResult<VoteQuestionDto>> UpvoteAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default) =>
        VoteAsync(initiatorId, questionId, VoteTypes.Upvote, BaseEventType.EntityUpvoted, cancellationToken);

    public Task<BaseResult<VoteQuestionDto>> DownvoteAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default) =>
        VoteAsync(initiatorId, questionId, VoteTypes.Downvote, BaseEventType.EntityDownvoted, cancellationToken);

    public async Task<BaseResult<VoteQuestionDto>> RemoveVoteAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default)
    {
        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);
        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);
        if (vote == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        unitOfWork.Votes.Remove(vote);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await producer.ProduceAsync(question.UserId, initiator.Id, question.Id, BaseEventType.EntityVoteRemoved,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    private async Task<BaseResult<VoteQuestionDto>> VoteAsync(long initiatorId, long questionId,
        VoteTypes voteTypeName, BaseEventType eventType, CancellationToken cancellationToken)
    {
        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Votes)
            .ThenInclude(x => x.VoteType)
            .FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);
        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator.Id == question.UserId)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.CannotVoteForOwnPost,
                (int)ErrorCodes.CannotVoteForOwnPost);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);

        var voteTypeNameString = voteTypeName.ToString();
        var voteType = await voteTypeRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == voteTypeNameString, cancellationToken);
        if (voteType == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteTypeNotFound, (int)ErrorCodes.VoteTypeNotFound);

        if (initiator.Reputation < voteType.MinReputationToVote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

        if (vote != null && vote.VoteType.Id == voteType.Id)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                (int)ErrorCodes.VoteAlreadyGiven);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        if (vote == null)
        {
            vote = new Vote
            {
                QuestionId = question.Id,
                UserId = initiator.Id,
                VoteType = voteType
            };

            await unitOfWork.Votes.CreateAsync(vote, cancellationToken);
        }
        else
        {
            vote.VoteType = voteType;
            unitOfWork.Votes.Update(vote);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await producer.ProduceAsync(question.UserId, initiator.Id, question.Id, eventType, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }
}