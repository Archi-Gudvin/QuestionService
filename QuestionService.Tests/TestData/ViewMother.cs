using View = QuestionService.Domain.Entities.View;

namespace QuestionService.Tests.TestData;

internal static class ViewMother
{
    public static IQueryable<View> GetViews()
    {
        return new[]
        {
            new View
            {
                Id = 1,
                QuestionId = 1,
                UserId = 1,
                UserIp = null,
                UserFingerprint = null
            },
            new View
            {
                Id = 2,
                QuestionId = 2,
                UserId = 1,
                UserIp = null,
                UserFingerprint = null
            },
            new View
            {
                Id = 3,
                QuestionId = 2,
                UserId = null,
                UserIp = "0.0.0.0",
                UserFingerprint = "testFingerprint3"
            },
            new View
            {
                Id = 4,
                QuestionId = 3,
                UserId = null,
                UserIp = "1.0.0.1",
                UserFingerprint = "testFingerprint4"
            },
            new View
            {
                Id = 5,
                QuestionId = 4,
                UserId = 2,
                UserIp = null,
                UserFingerprint = null
            }
        }.AsQueryable();
    }
}
