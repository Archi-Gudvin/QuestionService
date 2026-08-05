using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class TagDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Load_ExistingTagId_ReturnsTag()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<TagDataLoader>();
        const long tagId = 1;

        //Act
        var result = await dataLoader.LoadAsync(tagId);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Load_NonExistentTagId_ReturnsNull()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<TagDataLoader>();
        const long tagId = 0;

        //Act
        var result = await dataLoader.LoadAsync(tagId);

        //Assert
        Assert.Null(result);
    }
}