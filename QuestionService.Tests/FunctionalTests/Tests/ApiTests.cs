using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helpers;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task PutQuestion_InvalidClaims_ReturnsForbidden()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/question";
        var token = TokenHelper.GetRsaToken("testuser2", 2, []);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await HttpClient.PutAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Invalid claims", body);
    }

    [Fact]
    public async Task PostQuestion_MissingToken_ReturnsUnauthorized()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/question";

        //Act
        var response = await HttpClient.PostAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task PostTag_InsufficientRole_ReturnsForbidden()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("testuser1", 1, [
            new RoleDto { Name = "User" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        const string forbiddenUrl = "/api/v1.0/tag";

        //Act
        var response = await HttpClient.PostAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetSwagger_ValidRequest_ReturnsSuccess()
    {
        //Arrange
        const string swaggerUrl = "/swagger/v1/swagger.json";

        //Act
        var response = await HttpClient.GetAsync(swaggerUrl);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }
}