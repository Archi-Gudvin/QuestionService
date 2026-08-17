namespace QuestionService.Api.Dtos;

public record RequestEditQuestionDto(string Title, string Body, IReadOnlyCollection<string> TagNames);