using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupVoteTypeVoteDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ExistingVoteTypeId_ReturnsVotes()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteTypeVoteDataLoader>();
        const long voteTypeId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(voteTypeId);

        //Assert
        Assert.Equal(3, result.Length);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_NonExistentVoteTypeId_ReturnsEmptyResult()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteTypeVoteDataLoader>();
        const long voteTypeId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(voteTypeId);

        //Assert
        Assert.Empty(result);
    }
}