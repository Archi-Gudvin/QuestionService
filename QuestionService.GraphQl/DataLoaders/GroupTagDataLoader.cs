using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.GraphQl.DataLoaders.Base;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores tags by questions ids
/// </summary>
public class GroupTagDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedEntityDataLoader<Tag, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> FetchAsync(
        IServiceProvider serviceProvider, IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IGetTagService>().GetQuestionsTagsAsync(keys, cancellationToken);
}
