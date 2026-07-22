using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Results;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helpers;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests;

[Collection(nameof(TagServiceTests))]
[FunctionalTest]
public class TagServiceTests : SequentialFunctionalTest
{
    public TagServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [
            new RoleDto { Name = "User" },
            new RoleDto { Name = "Moderator" }
        ]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateTag_ValidTag_ReturnsCreated()
    {
        //Arrange
        var dto = new CreateTagDto("NewTag", "NewTagDescription");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CreateTag_TagNameTooLong_ReturnsBadRequest()
    {
        //Arrange
        var dto = new CreateTagDto("TooLongTagNameTooLongTagNameTooLongTagName", "NewTagDescription");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTagName, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateTag_ExistingTag_ReturnsOk()
    {
        //Arrange
        var dto = new TagDto(1, "NewTag", "NewTagDescription");

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateTag_NonExistentTag_ReturnsNotFound()
    {
        //Arrange
        var dto = new TagDto(0, "NewTag", "NewTagDescription");

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteTag_ExistingTag_ReturnsOk()
    {
        //Arrange
        const long tagId = 3;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/tag/{tagId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteTag_NonExistentTag_ReturnsNotFound()
    {
        //Arrange
        const long tagId = 0;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/tag/{tagId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}