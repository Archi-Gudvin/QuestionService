using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;

namespace QuestionService.Tests.TestData;

internal static class VoteTypeMother
{
    public static IQueryable<VoteType> GetVoteTypes()
    {
        return new[]
        {
            GetVoteTypeUpvote(),
            GetVoteTypeDownvote()
        }.AsQueryable();
    }

    public static VoteType GetVoteTypeUpvote()
    {
        return new VoteType
        {
            Id = 1,
            ReputationChange = 1,
            MinReputationToVote = 15,
            Name = nameof(VoteTypes.Upvote)
        };
    }

    public static VoteType GetVoteTypeDownvote()
    {
        return new VoteType
        {
            Id = 2,
            ReputationChange = -1,
            MinReputationToVote = 125,
            Name = nameof(VoteTypes.Downvote)
        };
    }
}
