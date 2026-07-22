using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using QuestionService.Api.Dtos;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Results;
using QuestionService.Tests.FunctionalTests.Base.Exception;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class ExceptionTests : ExceptionFunctionalTest
{
    public ExceptionTests(ExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [
            new RoleDto { Name = "User" }
        ]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task AskQuestion_TransactionCommitFailure_ReturnsInternalServerError()
    {
        //Arrange
        var dto = new AskQuestionDto("NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/question", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task EditQuestion_TransactionCommitFailure_ReturnsInternalServerError()
    {
        //Arrange
        const long questionId = 1;
        var dto = new RequestEditQuestionDto("NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/question/{questionId}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteQuestion_TransactionCommitFailure_ReturnsInternalServerError()
    {
        //Arrange
        const long questionId = 1;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/question/{questionId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<QuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DownvoteQuestion_TransactionCommitFailure_ReturnsInternalServerError()
    {
        //Arrange
        const long questionId = 2;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/{questionId}/downvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpvoteQuestion_TransactionCommitFailure_ReturnsInternalServerError()
    {
        //Arrange
        const long questionId = 3;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/{questionId}/upvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RemoveQuestionVote_TransactionCommitFailure_ReturnsInternalServerError()
    {
        //Arrange
        const long questionId = 2;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/question/{questionId}/vote");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetQuestionById_CacheReadFailure_ReturnsOk()
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
}