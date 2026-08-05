using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base.Exception.Outbox;

public class OutboxProcessorFailureFunctionalTest : IClassFixture<OutboxProcessorFailureFunctionalTestWebAppFactory>
{
    protected readonly IServiceProvider ServiceProvider;

    protected OutboxProcessorFailureFunctionalTest(OutboxProcessorFailureFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }
}
