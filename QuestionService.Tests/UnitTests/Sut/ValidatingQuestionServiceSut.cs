using FluentValidation;
using Moq;
using QuestionService.Application.Services.Decorators;
using QuestionService.Application.Validators;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Results;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class ValidatingQuestionServiceSut
{
    private readonly IQuestionService _validatingQuestionService;

    public readonly IQuestionService QuestionService = QuestionServiceFixture.GetQuestionServiceConfiguration();

    public readonly IValidator<IValidatableQuestion> Validator =
        ValidatorFixture<IValidatableQuestion>.GetValidator(new QuestionValidator());

    public ValidatingQuestionServiceSut()
    {
        _validatingQuestionService = new ValidatingQuestionService(Validator, QuestionService);
    }

    public IQuestionService GetService()
    {
        return _validatingQuestionService;
    }
}
