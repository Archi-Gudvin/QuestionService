using AutoMapper;
using FluentValidation;
using QuestionService.Application.Services;
using QuestionService.Application.Validators;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Tests.Mocks;
using QuestionService.Tests.UnitTests.Fixtures;

namespace QuestionService.Tests.UnitTests.Sut;

internal class TagServiceSut
{
    private readonly ITagService _tagService;

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = RepositoryMocks.GetMockTagRepository().Object;

    public readonly IValidator<IValidatableTag> Validator =
        ValidatorFixture<IValidatableTag>.GetValidator(new TagValidator());


    public TagServiceSut()
    {
        _tagService = new TagService(TagRepository, Mapper, Validator);
    }

    public ITagService GetService()
    {
        return _tagService;
    }
}
