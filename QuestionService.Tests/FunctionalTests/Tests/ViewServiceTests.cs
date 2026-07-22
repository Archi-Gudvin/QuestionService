using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Results;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class ViewServiceTests : BaseFunctionalTest
{
    public ViewServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [
            new RoleDto { Name = "User" }
        ]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task IncrementViews_ValidRequest_ReturnsNoContent()
    {
        //Arrange
        const long questionId = 1;
        HttpClient.DefaultRequestHeaders.Add("X-Fingerprint", "someFingerprint");
        HttpClient.DefaultRequestHeaders.Add("X-Forwarded-For", "127.0.0.1");

        //Act
        var response = await HttpClient.PostAsync($"api/v1.0/View/{questionId}", null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact]
    public async Task IncrementViews_MissingIpHeader_ReturnsBadRequest()
    {
        //Arrange
        const long questionId = 1;
        HttpClient.DefaultRequestHeaders.Add("X-Fingerprint", "someFingerprint");

        //Act
        var response = await HttpClient.PostAsync($"api/v1.0/View/{questionId}", null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("IP Address is not provided", body);
    }

    [Fact]
    public async Task IncrementViews_MissingFingerprintHeader_ReturnsBadRequest()
    {
        //Arrange
        const long questionId = 1;
        HttpClient.DefaultRequestHeaders.Add("X-Forwarded-For", "127.0.0.1");

        //Act
        var response = await HttpClient.PostAsync($"api/v1.0/View/{questionId}", null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Fingerprint is not provided", body);
    }

    [Fact]
    public async Task IncrementViews_FingerprintTooLong_ReturnsBadRequest()
    {
        //Arrange
        const long questionId = 1;
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Fingerprint",
            "tooLongFingerprinttooLongFingerprinttooLongFingerprinttooLongFingerprinttoo");
        HttpClient.DefaultRequestHeaders.Add("X-Forwarded-For", "127.0.0.1");

        //Act
        var response = await HttpClient.PostAsync($"api/v1.0/View/{questionId}", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
    }
}