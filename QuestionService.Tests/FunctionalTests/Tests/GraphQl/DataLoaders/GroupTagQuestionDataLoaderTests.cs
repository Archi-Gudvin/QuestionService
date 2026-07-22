using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class GroupTagQuestionDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadRequiredAsync_ExistingTagId_ReturnsGroupedQuestions()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupTagQuestionDataLoader>();
        const long tagId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(tagId);

        //Assert
        Assert.Equal(3, result.Length); // Tag with name ".NET" has 3 questions
    }

    [Fact]
    public async Task LoadRequiredAsync_NonExistentTagId_ReturnsEmptyResult()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupTagQuestionDataLoader>();
        const long tagId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(tagId);

        //Assert
        Assert.Empty(result);
    }
}