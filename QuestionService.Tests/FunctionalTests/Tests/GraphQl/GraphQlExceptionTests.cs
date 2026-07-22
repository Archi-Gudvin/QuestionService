using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using QuestionService.Application.Resources;
using QuestionService.Tests.FunctionalTests.Base.Exception.GraphQl;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl;

[FunctionalTest]
public class GraphQlExceptionTests(GraphQlExceptionFunctionalTestWebAppFactory factory)
    : GraphQlExceptionFunctionalTest(factory)
{
    [Fact]
    public async Task GetAll_UnhandledServiceException_ReturnsInternalServerError()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body)!;

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(result.Errors, x => x.Message.StartsWith(ErrorMessage.InternalServerError));
    }
}