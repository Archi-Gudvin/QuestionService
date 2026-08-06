using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;

namespace QuestionService.Tests.FunctionalTests.Base.Exception.GraphQl;

public class ExceptionGraphQlFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IBaseRepository<Question>>();
            services.AddScoped<IBaseRepository<Question>>(_ => null!); //Passing null to cause NullReferenceException
        });
    }
}