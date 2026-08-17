using FluentValidation;
using QuestionService.Application.Resources;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Validators;

public class QuestionValidator : AbstractValidator<IValidatableQuestion>
{
    public QuestionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessage.InvalidTitle)
            .MinimumLength(BusinessRules.TitleMinLength).WithMessage(ErrorMessage.InvalidTitle)
            .MaximumLength(BusinessRules.TitleMaxLength).WithMessage(ErrorMessage.InvalidTitle);

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage(ErrorMessage.InvalidBody)
            .MinimumLength(BusinessRules.BodyMinLength).WithMessage(ErrorMessage.InvalidBody)
            .MaximumLength(BusinessRules.BodyMaxLength).WithMessage(ErrorMessage.InvalidBody);

        RuleFor(x => x.TagNames)
            .NotNull().WithMessage(ErrorMessage.InvalidTags)
            .Must(x => x.Count is >= 1 and <= BusinessRules.MaxTagsCount)
            .WithMessage(ErrorMessage.InvalidTags);
    }
}