using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enums;
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

public class QuestionService(
    IUnitOfWork unitOfWork,
    IBaseRepository<Tag> tagRepository,
    IEntityProvider<UserDto> userProvider,
    IMapper mapper,
    IBaseEventProducer producer)
    : IQuestionService
{
    public async Task<BaseResult<QuestionDto>> AskQuestionAsync(long initiatorId, AskQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (user == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var tagNames = dto.TagNames.Distinct().ToArray();
        var tags = await tagRepository.GetAll().Where(x => tagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);
        if (tags.Count != tagNames.Length)
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

        var tagNames = dto.TagNames.Distinct().ToArray();
        var tags = await tagRepository.GetAll().Where(x => tagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);
        if (tags.Count != tagNames.Length)
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

    private static bool HasAccess(UserDto initiator, Question toQuestion)
    {
        return initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Admin))
               || initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Moderator))
               || toQuestion.UserId == initiator.Id;
    }
}