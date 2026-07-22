using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Tests.UnitTests.Sut;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class TagServiceTests
{
    [Fact]
    public async Task CreateTagAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        var dto = new CreateTagDto("NewTag", "NewTagDescription");

        //Act
        var result = await tagService.CreateTagAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CreateTagAsync_TooLongTagName_ReturnsInvalidTagName()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        var dto = new CreateTagDto("TooLongTagNameTooLongTagNameTooLongTagName", "NewTagDescription");

        //Act
        var result = await tagService.CreateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTagName, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task CreateTagAsync_ExistingTagName_ReturnsTagAlreadyExists()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        var dto = new CreateTagDto(".NET", "NewTagDescription");

        //Act
        var result = await tagService.CreateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateTagAsync_ValidData_ReturnsSuccess()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        var dto = new TagDto(1, ".NET", "NewTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateTagAsync_TooLongTagDescription_ReturnsInvalidTagDescription()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        var dto = new TagDto(1, ".NET",
            "TooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTagDescription, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateTagAsync_NonExistentTagId_ReturnsTagNotFound()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        var dto = new TagDto(0, "NewTag", "NewTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteTagAsync_ExistingTagId_ReturnsSuccess()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        const long tagId = 3;

        //Act
        var result = await tagService.DeleteTagAsync(tagId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteTagAsync_NonExistentTagId_ReturnsTagNotFound()
    {
        //Arrange
        var tagService = new TagServiceSut().GetService();
        const long tagId = 0;

        //Act
        var result = await tagService.DeleteTagAsync(tagId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}