using QuestionService.Domain.Interfaces.Validation;

namespace QuestionService.Domain.Dtos.Question;

public record AskQuestionDto(string Title, string Body, IReadOnlyCollection<string> TagNames) : IValidatableQuestion;