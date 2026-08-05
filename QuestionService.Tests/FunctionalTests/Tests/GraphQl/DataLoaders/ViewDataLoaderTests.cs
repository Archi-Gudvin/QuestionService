using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class ViewDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Load_ExistingViewId_ReturnsView()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<ViewDataLoader>();
        const long viewId = 1;

        //Act
        var result = await dataLoader.LoadAsync(viewId);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Load_NonExistentViewId_ReturnsNull()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<ViewDataLoader>();
        const long viewId = 0;

        //Act
        var result = await dataLoader.LoadAsync(viewId);

        //Assert
        Assert.Null(result);
    }
}