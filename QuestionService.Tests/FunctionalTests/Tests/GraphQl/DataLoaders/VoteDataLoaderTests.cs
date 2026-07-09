using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class VoteDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ExistingVoteKey_ReturnsVote()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<VoteDataLoader>();
        var dto = new VoteDto(2, 1);

        //Act
        var result = await dataLoader.LoadAsync(dto);

        //Assert
        Assert.NotNull(result);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_NonExistentVoteKey_ReturnsNull()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<VoteDataLoader>();
        var dto = new VoteDto(0, 0);

        //Act
        var result = await dataLoader.LoadAsync(dto);

        //Assert
        Assert.Null(result);
    }
}