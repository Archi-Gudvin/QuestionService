using QuestionService.Application.Enums;
using QuestionService.Application.Extensions;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetQuestionService(IQuestionCacheRepository cacheRepository, IGetQuestionService inner)
    : IGetQuestionService
{
    public QueryableResult<Question> GetAll()
    {
        return inner.GetAll();
    }

    public async Task<CollectionResult<Question>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var questions = (await cacheRepository.GetByIdsAsync(ids,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (questions.Length == 0) return CollectionResult<Question>.QuestionsNotFound(ids.Count);

        return CollectionResult<Question>.Success(questions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IReadOnlyCollection<long> tagIds, CancellationToken cancellationToken = default)
    {
        var groupedQuestions = (await cacheRepository.GetQuestionsWithTagsAsync(tagIds,
            async (idsToFetch, ct) => (await inner.GetQuestionsWithTagsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedQuestions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IReadOnlyCollection<long> userIds, CancellationToken cancellationToken = default)
    {
        var groupedQuestions = (await cacheRepository.GetUsersQuestionsAsync(userIds,
            async (idsToFetch, ct) => (await inner.GetUsersQuestionsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedQuestions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }
}