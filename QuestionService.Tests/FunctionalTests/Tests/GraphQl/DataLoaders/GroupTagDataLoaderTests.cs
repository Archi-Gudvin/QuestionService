using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupTagDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadRequiredAsync_ExistingQuestionId_ReturnsGroupedTags()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupTagDataLoader>();
        const long questionId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(questionId);

        //Assert
        Assert.Single(result); // Question with id 1 has 1 tag
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadRequiredAsync_NonExistentQuestionId_ReturnsEmptyResult()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupTagDataLoader>();
        const long questionId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(questionId);

        //Assert
        Assert.Empty(result);
    }
}