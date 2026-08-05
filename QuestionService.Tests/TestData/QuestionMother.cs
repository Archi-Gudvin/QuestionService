using QuestionService.Domain.Entities;

namespace QuestionService.Tests.TestData;

internal static class QuestionMother
{
    public static IQueryable<Question> GetQuestions()
    {
        return new Question[]
        {
            new()
            {
                Id = 1,
                Title = "question1",
                Body = "questionBody1",
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [TagMother.GetTagDotNet()],
                Votes = [],
                Views = ViewMother.GetViews().Where(x => x.QuestionId == 1).ToList(),
                Enabled = true
            },
            new()
            {
                Id = 2,
                Title = "question2",
                Body = "questionBody2",
                UserId = 2,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [TagMother.GetTagDotNet(), TagMother.GetTagJava()],
                Votes = VoteMother.GetVotes().Where(x => x.QuestionId == 2).ToList(),
                Views = ViewMother.GetViews().Where(x => x.QuestionId == 2).ToList(),
                Enabled = true
            },
            new()
            {
                Id = 3,
                Title = "question3",
                Body = "questionBody3",
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [TagMother.GetTagDotNet(), TagMother.GetTagJava(), TagMother.GetTagPython()],
                Votes = VoteMother.GetVotes().Where(x => x.QuestionId == 3).ToList(),
                Views = ViewMother.GetViews().Where(x => x.QuestionId == 3).ToList(),
                Enabled = true
            },
            new() // Question without tags (not possible)
            {
                Id = 4,
                Title = "question4",
                Body = "questionBody4",
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [],
                Votes = [],
                Views = ViewMother.GetViews().Where(x => x.QuestionId == 4).ToList(),
                Enabled = true
            },
            new()
            {
                Id = 5,
                Title = "question5",
                Body = "questionBody5",
                UserId = 2,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [TagMother.GetTagPython()],
                Votes = [],
                Views = ViewMother.GetViews().Where(x => x.QuestionId == 5).ToList(),
                Enabled = false // Disabled question
            }
        }.AsQueryable();
    }
}
