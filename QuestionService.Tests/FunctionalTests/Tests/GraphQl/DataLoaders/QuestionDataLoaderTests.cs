using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class QuestionDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Load_ExistingQuestionId_ReturnsQuestion()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<QuestionDataLoader>();
        const long questionId = 1;

        //Act
        var result = await dataLoader.LoadAsync(questionId);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Load_NonExistentQuestionId_ReturnsNull()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<QuestionDataLoader>();
        const long questionId = 0;

        //Act
        var result = await dataLoader.LoadAsync(questionId);

        //Assert
        Assert.Null(result);
    }
}