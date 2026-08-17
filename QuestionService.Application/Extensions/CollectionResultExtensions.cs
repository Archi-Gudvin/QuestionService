using QuestionService.Application.Enums;
using QuestionService.Application.Resources;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Extensions;

public static class CollectionResultExtensions
{
    extension<T>(CollectionResult<T>)
    {
        public static CollectionResult<T> QuestionsNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.QuestionsNotFound, (int)ErrorCodes.QuestionsNotFound)
        };

        public static CollectionResult<T> TagsNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound)
        };

        public static CollectionResult<T> VotesNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound)
        };

        public static CollectionResult<T> VoteTypesNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.VoteTypeNotFound, (int)ErrorCodes.VoteTypeNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.VoteTypesNotFound, (int)ErrorCodes.VoteTypesNotFound)
        };

        public static CollectionResult<T> ViewsNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.ViewNotFound, (int)ErrorCodes.ViewNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.ViewsNotFound, (int)ErrorCodes.ViewsNotFound)
        };
    }
}