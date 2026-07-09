using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuestionService.Application.Services;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using QuestionService.Tests.FunctionalTests.Helper;
using StackExchange.Redis;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class CacheGetServicesTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    // Only functional tests are provided for cache services' success scenarios.
    // This is because cache data mirrors the database, and manually copying test DB data into multiple cache keys/values is impractical and confusing.
    // In functional tests, data is automatically copied from the DB to the cache as needed, following all key/value rules.

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionById_CacheHit_ReturnsQuestionWithTags()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestQuestionByIdQuery(2) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // 2nd request fetches data from cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdAndNameResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Question);
        Assert.NotNull(result.Data.Question.Tags);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionById_NonExistentIdCacheHit_ReturnsNull()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestQuestionByIdQuery(0) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // 2nd request fetches data from cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdAndNameResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.Question);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionById_CorruptedCacheEntry_ReturnsQuestionWithTags()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDatabase>();
        var requestBody = new { query = GraphQlHelper.RequestQuestionByIdQuery(2) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // Simulate a wrong entry in the cache
        await cache.StringSetAsync("tag:1", "Wrong data");
        // 2nd request fetches data from the cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdAndNameResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Question);
        Assert.NotNull(result.Data.Question.Tags);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionsTagsAsync_NonExistentQuestionId_ReturnsEmptyCollection()
    {
        //Arrange
        const long questionId = 0;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITagCacheRepository>();
        // Inner service is not in the DI
        var inner = ActivatorUtilities.CreateInstance<GetTagService>(scope.ServiceProvider);
        var fetch = async (IEnumerable<long> idsToFetch, CancellationToken ct) =>
            (await inner.GetQuestionsTagsAsync(idsToFetch, ct)).Data ?? [];

        //Act
        // The first call marks the user as null in the cache
        await repository.GetQuestionsTagsAsync([questionId], fetch);
        // The second call fetches the null entry from the cache
        var result = await repository.GetQuestionsTagsAsync([questionId], fetch);

        //Assert
        Assert.Empty(result);
    }
}