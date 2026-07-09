using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupVoteDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ExistingQuestionId_ReturnsVotes()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteDataLoader>();
        const long questionId = 2;

        //Act
        var result = await dataLoader.LoadRequiredAsync(questionId);

        //Assert
        Assert.Equal(2, result.Length); // Question with id 2 has 2 votes
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_NonExistentQuestionId_ReturnsEmptyResult()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteDataLoader>();
        const long questionId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(questionId);

        //Assert
        Assert.Empty(result);
    }
}