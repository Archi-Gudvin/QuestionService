using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.BackgroundJobs.Jobs;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helpers;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests;

[Collection(nameof(SyncViewsJobTests))]
[FunctionalTest]
public class SyncViewsJobTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Fact]
    public async Task RunAsync_PendingViewsInCache_ReturnsSuccess()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheProvider>();
        var syncViewsJob = ActivatorUtilities.CreateInstance<SyncViewsJob>(scope.ServiceProvider);
        var viewRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<View>>();

        await cache.InsertViews();

        //Act
        await syncViewsJob.RunAsync();

        //Assert
        var count = await viewRepository.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(11, count); // Total 11 views including new ones (consider views of the disabled questions)
    }

    [Fact]
    public async Task RunAsync_InvalidCachedViews_ReturnsNoSyncedViews()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheProvider>();
        var syncViewsJob = ActivatorUtilities.CreateInstance<SyncViewsJob>(scope.ServiceProvider);
        var viewRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<View>>();

        await cache.InsertInvalidValuesViews();

        //Act
        await syncViewsJob.RunAsync();

        //Assert
        var count = await viewRepository.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(4, count); // Total 4 views including new ones (consider views of the disabled questions)
    }
}