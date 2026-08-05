using FluentValidation;
using QuestionService.Application.Enum;
using QuestionService.Application.Helpers;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Decorators;

public class ValidatingQuestionService(IValidator<IValidatableQuestion> validator, IQuestionService inner)
    : IQuestionService
{
    public async Task<BaseResult<QuestionDto>> AskQuestionAsync(long initiatorId, AskQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateWithMessageAsync(dto, cancellationToken);
        return validation.IsValid
            ? await inner.AskQuestionAsync(initiatorId, dto, cancellationToken)
            : BaseResult<QuestionDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);
    }

    public async Task<BaseResult<QuestionDto>> EditQuestionAsync(long initiatorId, EditQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateWithMessageAsync(dto, cancellationToken);
        return validation.IsValid
            ? await inner.EditQuestionAsync(initiatorId, dto, cancellationToken)
            : BaseResult<QuestionDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);
    }

    public Task<BaseResult<QuestionDto>> DeleteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default) =>
        inner.DeleteQuestionAsync(initiatorId, questionId, cancellationToken);
}