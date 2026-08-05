using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.DAL;
using QuestionService.Domain.Entities;
using QuestionService.Tests.TestData;

namespace QuestionService.Tests.FunctionalTests.Configurations;

internal static class PrepDb
{
    public static void PrepPopulation(this IServiceScope serviceScope)
    {
        var questions = QuestionMother.GetQuestions()
            // Real question always has at least 1 tag
            .Where(x => x.Tags.Count >= 1)
            .Select(x => new Question
            {
                Title = x.Title,
                Body = x.Body,
                UserId = x.UserId,
                CreatedAt = x.CreatedAt,
                LastModifiedAt = x.LastModifiedAt,
                Enabled = x.Enabled
            });

        var tags = TagMother.GetTags();
        var questionTags = TagMother.GetQuestionTags();
        var votes = VoteMother.GetVotes();
        var voteTypes = VoteTypeMother.GetVoteTypes();
        var views = ViewMother.GetViews();

        views.ForEach(x => x.Id = 0);
        tags.ForEach(x => x.Id = 0);
        voteTypes.ForEach(x => x.Id = 0);
        votes.ForEach(x => x.VoteType = null!);

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        dbContext.Set<Question>().AddRange(questions);
        dbContext.Set<Tag>().AddRange(tags);
        dbContext.Set<VoteType>().AddRange(voteTypes);
        dbContext.Set<Vote>().AddRange(votes);
        dbContext.Set<View>().AddRange(views);

        dbContext.SaveChanges();

        // Adding many-to-many entities
        dbContext.Set<QuestionTag>().AddRange(questionTags);

        dbContext.SaveChanges();
    }
}