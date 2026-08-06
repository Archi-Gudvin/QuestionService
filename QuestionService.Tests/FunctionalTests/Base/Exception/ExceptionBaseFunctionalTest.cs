using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base.Exception;

public class ExceptionBaseFunctionalTest : IClassFixture<ExceptionFunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;
    protected readonly IServiceProvider ServiceProvider;

    protected ExceptionBaseFunctionalTest(ExceptionFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
        ServiceProvider = factory.Services;
    }
}