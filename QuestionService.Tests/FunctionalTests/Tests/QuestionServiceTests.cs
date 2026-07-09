using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using QuestionService.Api.Dtos;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Results;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

[Collection(nameof(QuestionServiceTests))]
public class QuestionServiceTests : SequentialFunctionalTest
{
    public QuestionServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [new RoleDto { Name = "User" }]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AskQuestion_ValidQuestion_ReturnsCreated()
    {
        //Arrange
        var dto = new AskQuestionDto("NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/question", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AskQuestion_NonExistentUser_ReturnsNotFound()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("WrongUser", 0, [
            new RoleDto
            {
                Id = 1,
                Name = "User"
            }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new AskQuestionDto("NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/question", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task EditQuestion_ExistingQuestion_ReturnsOk()
    {
        //Arrange
        const long questionId = 1;
        var dto = new RequestEditQuestionDto("NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/question/{questionId}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task EditQuestion_NonExistentQuestion_ReturnsNotFound()
    {
        //Arrange
        const long questionId = 0;
        var dto = new RequestEditQuestionDto("NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/question/{questionId}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteQuestion_ExistingQuestion_ReturnsOk()
    {
        //Arrange
        const long questionId = 1;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/question/{questionId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteQuestion_NonExistentQuestion_ReturnsNotFound()
    {
        //Arrange
        const long questionId = 0;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/question/{questionId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DownvoteQuestion_ExistingQuestion_ReturnsOk()
    {
        //Arrange
        const long questionId = 2;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/{questionId}/downvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DownvoteQuestion_NonExistentQuestion_ReturnsNotFound()
    {
        //Arrange
        const long questionId = 0;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/{questionId}/downvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpvoteQuestion_ExistingQuestion_ReturnsOk()
    {
        //Arrange
        const long questionId = 3;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/{questionId}/upvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpvoteQuestion_NonExistentQuestion_ReturnsNotFound()
    {
        //Arrange
        const long questionId = 0;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/{questionId}/upvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RemoveQuestionVote_ExistingVote_ReturnsOk()
    {
        //Arrange
        const long questionId = 2;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/question/{questionId}/vote");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RemoveQuestionVote_NonExistentQuestion_ReturnsNotFound()
    {
        //Arrange
        const long questionId = 0;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/question/{questionId}/vote");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}