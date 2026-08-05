using QuestionService.Domain.Entities;

namespace QuestionService.Tests.TestData;

internal static class VoteMother
{
    public static IQueryable<Vote> GetVotes()
    {
        return new[]
        {
            GetUpvote(1, 2), GetUpvote(2, 1), GetDownvote(4, 2),
            GetUpvote(2, 3), GetDownvote(1, 3), GetDownvote(4, 3)
        }.AsQueryable();
    }

    public static Vote GetUpvote(long userId, long questionId)
    {
        return new Vote
        {
            UserId = userId,
            QuestionId = questionId,
            VoteTypeId = VoteTypeMother.GetVoteTypeUpvote().Id,
            VoteType = VoteTypeMother.GetVoteTypeUpvote()
        };
    }

    public static Vote GetDownvote(long userId, long questionId)
    {
        return new Vote
        {
            UserId = userId,
            QuestionId = questionId,
            VoteTypeId = VoteTypeMother.GetVoteTypeDownvote().Id,
            VoteType = VoteTypeMother.GetVoteTypeDownvote()
        };
    }
}
