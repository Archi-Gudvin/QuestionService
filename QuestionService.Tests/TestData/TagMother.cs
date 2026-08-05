using QuestionService.Domain.Entities;

namespace QuestionService.Tests.TestData;

internal static class TagMother
{
    public static IQueryable<Tag> GetTags()
    {
        return new[]
        {
            GetTagDotNet(),
            GetTagJava(),
            GetTagPython()
        }.AsQueryable();
    }

    public static IQueryable<QuestionTag> GetQuestionTags()
    {
        return new[]
        {
            new QuestionTag
            {
                QuestionId = 1,
                TagId = 1
            },
            new QuestionTag
            {
                QuestionId = 2,
                TagId = 1
            },
            new QuestionTag
            {
                QuestionId = 2,
                TagId = 2
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagId = 1
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagId = 2
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagId = 3
            }
        }.AsQueryable();
    }

    public static Tag GetTagDotNet()
    {
        return new Tag
        {
            Id = 1,
            Name = ".NET",
            Description = ".NET is a secure, reliable, and high-performance application platform."
        };
    }

    public static Tag GetTagJava()
    {
        return new Tag
        {
            Id = 2,
            Name = "Java",
            Description = "Java is a high-level, general-purpose, memory-safe, object-oriented programming language."
        };
    }

    public static Tag GetTagPython()
    {
        return new Tag
        {
            Id = 3,
            Name = "Python",
            Description = "Python is a high-level, general-purpose programming language."
        };
    }
}
