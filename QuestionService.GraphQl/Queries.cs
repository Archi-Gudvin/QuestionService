using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.GraphQl.Helpers;
using QuestionService.GraphQl.Middlewares;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of paginated questions")]
    [UseCursorPagingValidationMiddleware]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Question>> GetQuestionsAsync([Service] IGetQuestionService questionService,
        CancellationToken cancellationToken)
    {
        var result = await questionService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a question by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Question?> GetQuestionAsync(long id, QuestionDataLoader questionLoader,
        CancellationToken cancellationToken)
    {
        var question = await questionLoader.LoadAsync(id, cancellationToken);

        return question;
    }

    [GraphQLDescription("Returns a list of paginated tags")]
    [UseCursorPagingValidationMiddleware]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Tag>> GetTagsAsync([Service] IGetTagService tagService,
        CancellationToken cancellationToken)
    {
        var result = await tagService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a tag by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Tag?> GetTagAsync(long id, TagDataLoader tagLoader, CancellationToken cancellationToken)
    {
        var tag = await tagLoader.LoadAsync(id, cancellationToken);

        return tag;
    }

    [GraphQLDescription("Returns a list of paginated votes")]
    [UseOffsetPagingValidationMiddleware]
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Vote>> GetQuestionVotesAsync([Service] IGetVoteService voteService,
        CancellationToken cancellationToken)
    {
        var result = await voteService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a vote by id of the question that was voted and the user that voted")]
    [UseFiltering]
    [UseSorting]
    public async Task<Vote?> GetQuestionVoteAsync(long questionId, long userId, VoteDataLoader voteLoader,
        CancellationToken cancellationToken)
    {
        var dto = new VoteDto(questionId, userId);
        var vote = await voteLoader.LoadAsync(dto, cancellationToken);

        return vote;
    }

    [GraphQLDescription("Returns a list of paginated votes types")]
    [UseOffsetPagingValidationMiddleware]
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<VoteType>> GetQuestionVoteTypesAsync([Service] IGetVoteTypeService voteTypeService,
        CancellationToken cancellationToken)
    {
        var result = await voteTypeService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a vote type by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<VoteType?> GetQuestionVoteTypeAsync(long id, VoteTypeDataLoader voteTypeLoader,
        CancellationToken cancellationToken)
    {
        var voteType = await voteTypeLoader.LoadAsync(id, cancellationToken);

        return voteType;
    }

    [GraphQLDescription("Returns a list of paginated views")]
    [UseOffsetPagingValidationMiddleware]
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<View>> GetQuestionViewsAsync([Service] IGetViewService viewService,
        CancellationToken cancellationToken)
    {
        var result = await viewService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a view by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<View?> GetQuestionViewAsync(long id, ViewDataLoader viewLoader,
        CancellationToken cancellationToken)
    {
        var view = await viewLoader.LoadAsync(id, cancellationToken);

        return view;
    }
}