using Grpc.Core;
using Grpc.Net.Client;
using QuestionService.Application.Resources;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class GrpcTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionById_ExistingQuestionId_ReturnsQuestion()
    {
        //Arrange
        const long questionId = 1;
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new QuestionService.QuestionServiceClient(channel);

        //Act
        var question = await client.GetQuestionByIdAsync(new GetQuestionByIdRequest { Id = questionId });

        //Assert
        Assert.NotNull(question);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionById_NonExistentQuestionId_ThrowsRpcException()
    {
        //Arrange
        const long questionId = 0;
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new QuestionService.QuestionServiceClient(channel);

        //Act
        var request = async () => await client.GetQuestionByIdAsync(new GetQuestionByIdRequest { Id = questionId });

        //Assert
        var exception = await Assert.ThrowsAsync<RpcException>(request);
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Equal(ErrorMessage.QuestionNotFound, exception.Status.Detail);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionsById_MixOfExistingAndNonExistentIds_ReturnsExistingQuestions()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new QuestionService.QuestionServiceClient(channel);

        var questionRequest = new GetQuestionsByIdsRequest();
        questionRequest.Ids.AddRange(questionIds);

        //Act
        var response = await client.GetQuestionsByIdsAsync(questionRequest);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Questions.Count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionsById_AllNonExistentIds_ThrowsRpcException()
    {
        //Arrange
        var questionIds = new List<long> { 0, -1 };
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new QuestionService.QuestionServiceClient(channel);

        var request = new GetQuestionsByIdsRequest();
        request.Ids.AddRange(questionIds);

        //Act
        var questionsRequest = async () => await client.GetQuestionsByIdsAsync(request);

        //Assert
        var exception = await Assert.ThrowsAsync<RpcException>(questionsRequest);
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Equal(ErrorMessage.QuestionsNotFound, exception.Status.Detail);
    }
}