using System.Net;
using QuestionService.Tests.UnitTests.Configurations;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GrpcStatusMappingHandlerTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task SendAsync_GrpcStatusOk_ReturnsOk()
    {
        //Arrange
        var handler = new TestableGrpcStatusMappingHandler();
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        //Act
        var response = await handler.SendAsync(request, CancellationToken.None);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SendAsync_GrpcStatusInvalidArgument_ReturnsBadRequest()
    {
        //Arrange
        var handler = new TestableGrpcStatusMappingHandler("3");
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        //Act
        var response = await handler.SendAsync(request, CancellationToken.None);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}