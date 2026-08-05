using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enum;
using QuestionService.Application.Helpers;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class QuestionService(
    IUnitOfWork unitOfWork,
    IBaseRepository<Tag> tagRepository,
    IBaseRepository<VoteType> voteTypeRepository,
    IEntityProvider<UserDto> userProvider,
    IMapper mapper,
    IBaseEventProducer producer,
    IValidator<IValidatableQuestion> questionValidator)
    : IQuestionService
{
    public async Task<BaseResult<QuestionDto>> AskQuestionAsync(long initiatorId, AskQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await questionValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<QuestionDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);


        var user = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (user == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var tags = await tagRepository.GetAll().Where(x => dto.TagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);
        if (tags.Count != dto.TagNames.Count())
            return BaseResult<QuestionDto>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var question = mapper.Map<Question>(dto);
        question.UserId = initiatorId;
        question.Tags = tags;

        await unitOfWork.Questions.CreateAsync(question, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var questionDto = mapper.Map<QuestionDto>(question);
        return BaseResult<QuestionDto>.Success(questionDto);
    }

    public async Task<BaseResult<QuestionDto>> EditQuestionAsync(long initiatorId, EditQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await questionValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<QuestionDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);

        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        var tags = await tagRepository.GetAll().Where(x => dto.TagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);
        if (tags.Count != dto.TagNames.Count())
            return BaseResult<QuestionDto>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        mapper.Map(dto, question);
        question.Tags = tags;

        unitOfWork.Questions.Update(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<QuestionDto>> DeleteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default)
    {
        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);
        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        question.Enabled = false;
        unitOfWork.Questions.Update(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await producer.ProduceAsync(question.UserId, initiator.Id, question.Id, BaseEventType.EntityDeleted,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<VoteQuestionDto>> UpvoteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default)
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

        var voteType = await voteTypeRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == nameof(VoteTypes.Upvote), cancellationToken);
        if (voteType == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteTypeNotFound, (int)ErrorCodes.VoteTypeNotFound);

        if (initiator.Reputation < voteType.MinReputationToVote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

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
            if (vote.VoteType.Id == voteType.Id)
                return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                    (int)ErrorCodes.VoteAlreadyGiven);

            vote.VoteType = voteType;
            unitOfWork.Votes.Update(vote);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await producer.ProduceAsync(question.UserId, initiator.Id, question.Id, BaseEventType.EntityUpvoted,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    public async Task<BaseResult<VoteQuestionDto>> DownvoteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default)
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

        var voteType = await voteTypeRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == nameof(VoteTypes.Downvote), cancellationToken);
        if (voteType == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteTypeNotFound, (int)ErrorCodes.VoteTypeNotFound);

        if (initiator.Reputation < voteType.MinReputationToVote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

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
            if (vote.VoteType.Id == voteType.Id)
                return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                    (int)ErrorCodes.VoteAlreadyGiven);

            vote.VoteType = voteType;
            unitOfWork.Votes.Update(vote);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await producer.ProduceAsync(question.UserId, initiator.Id, question.Id, BaseEventType.EntityDownvoted,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    public async Task<BaseResult<VoteQuestionDto>> RemoveQuestionVoteAsync(long initiatorId, long questionId,
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

    private static bool HasAccess(UserDto initiator, Question toQuestion)
    {
        return initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Admin))
               || initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Moderator))
               || toQuestion.UserId == initiator.Id;
    }
}
