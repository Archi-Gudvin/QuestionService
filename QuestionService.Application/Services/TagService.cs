using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enums;
using QuestionService.Application.Extensions;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class TagService(IBaseRepository<Tag> tagRepository, IMapper mapper, IValidator<IValidatableTag> tagValidator)
    : ITagService
{
    public async Task<BaseResult<TagDto>> CreateTagAsync(CreateTagDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await tagValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<TagDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);


        var tag = await tagRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name, cancellationToken);
        if (tag != null)
            return BaseResult<TagDto>.Failure(ErrorMessage.TagAlreadyExists, (int)ErrorCodes.TagAlreadyExists);

        tag = mapper.Map<Tag>(dto);

        await tagRepository.CreateAsync(tag, cancellationToken);
        await tagRepository.SaveChangesAsync(cancellationToken);

        return BaseResult<TagDto>.Success(mapper.Map<TagDto>(tag));
    }

    public async Task<BaseResult<TagDto>> UpdateTagAsync(TagDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await tagValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<TagDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);

        var tag = await tagRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (tag == null)
            return BaseResult<TagDto>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound);

        mapper.Map(dto, tag);

        tagRepository.Update(tag);

        await tagRepository.SaveChangesAsync(cancellationToken);

        return BaseResult<TagDto>.Success(dto);
    }

    public async Task<BaseResult<TagDto>> DeleteTagAsync(long id, CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tag == null)
            return BaseResult<TagDto>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound);

        tagRepository.Remove(tag);
        await tagRepository.SaveChangesAsync(cancellationToken);

        return BaseResult<TagDto>.Success(mapper.Map<TagDto>(tag));
    }
}