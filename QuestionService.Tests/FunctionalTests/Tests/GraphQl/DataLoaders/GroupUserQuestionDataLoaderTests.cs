using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupUserQuestionDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ExistingUserId_ReturnsSingleQuestion()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserQuestionDataLoader>();
        const long userId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Single(result); // User with id 1 has 1 question
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_NonExistentUserId_ReturnsEmptyResult()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserQuestionDataLoader>();
        const long userId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Empty(result);
    }
}