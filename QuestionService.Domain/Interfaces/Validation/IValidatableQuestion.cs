namespace QuestionService.Domain.Interfaces.Validation;

public interface IValidatableQuestion
{
    public string Title { get; }
    public string Body { get; }
    public IReadOnlyCollection<string> TagNames { get; }
}