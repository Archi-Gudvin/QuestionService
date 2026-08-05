using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using QuestionService.Outbox;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Tests.Support;

namespace QuestionService.Tests.FunctionalTests.Base.Exception.Outbox;

/// <summary>
///     Makes the first OutboxBackgroundService tick's IOutboxProcessor call throw, then delegates every
///     later call to a real processor. Used to prove one transient failure no longer stops the background
///     service for the rest of the process lifetime.
/// </summary>
public class OutboxProcessorFailureFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private int _callCount;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IOutboxProcessor>();
            services.AddScoped<IOutboxProcessor>(provider =>
            {
                var realProcessor = ActivatorUtilities.CreateInstance<OutboxProcessor>(provider);
                var mockProcessor = new Mock<IOutboxProcessor>();

                mockProcessor.Setup(x => x.ProcessOutboxMessagesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .Returns<int, CancellationToken>((batchSize, cancellationToken) =>
                    {
                        if (Interlocked.Increment(ref _callCount) == 1)
                            throw new TestException();

                        return realProcessor.ProcessOutboxMessagesAsync(batchSize, cancellationToken);
                    });

                return mockProcessor.Object;
            });
        });
    }
}
