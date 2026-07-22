using System.Net;
using QuestionService.GrpcClient.Handlers;

namespace QuestionService.Tests.UnitTests.Fixtures;

internal class TestableGrpcStatusMappingHandler : GrpcStatusMappingHandler
{
    public TestableGrpcStatusMappingHandler(string? grpcStatus = null)
    {
        InnerHandler = new TestInnerHandler(grpcStatus);
    }

    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return base.SendAsync(request, cancellationToken);
    }

    private class TestInnerHandler(string? grpcStatus) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            if (!string.IsNullOrEmpty(grpcStatus)) response.Headers.Add("grpc-status", grpcStatus);
            return Task.FromResult(response);
        }
    }
}