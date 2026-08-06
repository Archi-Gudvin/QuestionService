using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enums;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetQuestionService(
    IBaseRepository<Question> questionRepository,
    IBaseRepository<Tag> tagRepository)
    : IGetQuestionService
{
    public Task<QueryableResult<Question>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var questions = questionRepository.GetAll();

        // Since there can be no questions, it is not exception to have no questions
        return Task.FromResult(QueryableResult<Question>.Success(questions));
    }

    public async Task<CollectionResult<Question>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var questions = await questionRepository.GetAll().Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        if (questions.Length == 0)
            return ids.Count switch
            {
                <= 1 => CollectionResult<Question>.Failure(ErrorMessage.QuestionNotFound,
                    (int)ErrorCodes.QuestionNotFound),
                > 1 => CollectionResult<Question>.Failure(ErrorMessage.QuestionsNotFound,
                    (int)ErrorCodes.QuestionsNotFound)
            };

        return CollectionResult<Question>.Success(questions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IReadOnlyCollection<long> tagIds, CancellationToken cancellationToken = default)
    {
        var groupedQuestions = await tagRepository.GetAll()
            .Where(x => tagIds.Contains(x.Id))
            .Include(x => x.Questions)
            .Select(x => new KeyValuePair<long, IEnumerable<Question>>(x.Id, x.Questions))
            .ToArrayAsync(cancellationToken);

        if (groupedQuestions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IReadOnlyCollection<long> userIds, CancellationToken cancellationToken = default)
    {
        var questions = (await questionRepository.GetAll()
                .Where(x => userIds.Contains(x.UserId))
                .GroupBy(x => x.UserId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Question>>(x.Key, x.ToArray()))
            .ToArray();

        if (questions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(questions);
    }
}