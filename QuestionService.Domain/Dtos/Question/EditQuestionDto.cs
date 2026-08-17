using QuestionService.Domain.Interfaces.Validation;

namespace QuestionService.Domain.Dtos.Question;

public record EditQuestionDto(long Id, string Title, string Body, IReadOnlyCollection<string> TagNames) : IValidatableQuestion;