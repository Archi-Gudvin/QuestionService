using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Application.Mappings;
using QuestionService.Application.Services.Cache;
using QuestionService.Application.Services.Decorators;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddAutoMapper(typeof(QuestionMapping));
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssemblyOf<Services.QuestionService>()
            .AddClasses(c => c.InExactNamespaceOf<Services.QuestionService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssemblyContaining<Services.QuestionService>();

        services.Decorate<IGetQuestionService, CacheGetQuestionService>();
        services.Decorate<IGetVoteService, CacheGetVoteService>();
        services.Decorate<IGetVoteTypeService, CacheGetVoteTypeService>();
        services.Decorate<IGetTagService, CacheGetTagService>();
        services.Decorate<IGetViewService, CacheGetViewService>();

        services.Decorate<IQuestionService, ValidatingQuestionService>();
    }
}