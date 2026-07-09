using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuestionService.Application.Resources;
using QuestionService.DAL;
using QuestionService.Domain.Entities;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl;

[Collection(nameof(GraphQlSequentialTests))]
public class GraphQlSequentialTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetTags_NoTagsInDb_ReturnsTagsNotFoundError()
    {
        //Arrange
        await DeleteTagsAsync();
        var requestBody = new { query = GraphQlHelper.RequestQuestionsWithTagsQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body)!;

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(result.Errors, x => x.Message == ErrorMessage.TagsNotFound);
    }

    private async Task DeleteTagsAsync()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Set<Tag>().ExecuteDeleteAsync();

        await dbContext.SaveChangesAsync();
    }
}