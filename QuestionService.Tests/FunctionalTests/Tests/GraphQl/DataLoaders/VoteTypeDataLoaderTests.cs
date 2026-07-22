using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class VoteTypeDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Load_ExistingVoteTypeId_ReturnsVoteType()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<VoteTypeDataLoader>();
        const long voteTypeId = 1;

        //Act
        var result = await dataLoader.LoadAsync(voteTypeId);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Load_NonExistentVoteTypeId_ReturnsNull()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<VoteTypeDataLoader>();
        const long voteTypeId = 0;

        //Act
        var result = await dataLoader.LoadAsync(voteTypeId);

        //Assert
        Assert.Null(result);
    }
}