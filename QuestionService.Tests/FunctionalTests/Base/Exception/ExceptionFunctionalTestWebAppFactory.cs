using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using QuestionService.DAL.Repositories;
using QuestionService.Domain.Interfaces.Database;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Interfaces.TopicProducer;
using QuestionService.Tests.Support;

namespace QuestionService.Tests.FunctionalTests.Base.Exception;

public class ExceptionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private static IMock<ITransaction> GetExceptionMockTransaction(ITransaction originalTransaction)
    {
        var mockTransaction = new Mock<ITransaction>();

        mockTransaction.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(originalTransaction.RollbackAsync);
        mockTransaction.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new TestException());

        return mockTransaction;
    }

    private static async Task<IMock<IUnitOfWork>> GetExceptionMockUnitOfWork(IUnitOfWork originalUnitOfWork)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var originalTransaction = await originalUnitOfWork.BeginTransactionAsync();
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(originalUnitOfWork.SaveChangesAsync);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetExceptionMockTransaction(originalTransaction).Object);
        mockUnitOfWork.Setup(x => x.Questions).Returns(originalUnitOfWork.Questions);
        mockUnitOfWork.Setup(x => x.Votes).Returns(originalUnitOfWork.Votes);

        return mockUnitOfWork;
    }

    private static IMock<ICacheProvider> GetExceptionMockCacheProvider()
    {
        var mockDatabase = new Mock<ICacheProvider>();

        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<IEnumerable<KeyValuePair<string, It.IsAnyType>>>(),
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TestException());

        mockDatabase.Setup(x =>
                x.GetJsonParsedAsync<It.IsAnyType>(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TestException());

        return mockDatabase;
    }

    private static IMock<ITopicProducerResolver> GetExceptionTopicProducerResolver()
    {
        var mockResolver = new Mock<ITopicProducerResolver>();

        mockResolver.Setup(x => x.GetProducerForType(It.IsAny<Type>())).Throws(new TestException());

        return mockResolver;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IUnitOfWork>();
            services.AddScoped<IUnitOfWork>(provider =>
            {
                //the dependencies from service provider only apply for this current scope
                //that is why we have to use ActivatorUtilities to transfer dependencies from this scope to callers' scope
                var unitOfWork = ActivatorUtilities.CreateInstance<UnitOfWork>(provider);
                var exceptionUnitOfWork = GetExceptionMockUnitOfWork(unitOfWork).GetAwaiter().GetResult().Object;

                return exceptionUnitOfWork;
            });

            services.RemoveAll<ITopicProducerResolver>();
            services.AddScoped<ITopicProducerResolver>(_ =>
            {
                var exceptionTopicProducerResolver = GetExceptionTopicProducerResolver().Object;

                return exceptionTopicProducerResolver;
            });

            services.RemoveAll<ICacheProvider>();
            services.AddScoped<ICacheProvider>(_ =>
            {
                var exceptionRedisDatabase = GetExceptionMockCacheProvider().Object;
                return exceptionRedisDatabase;
            });
        });
    }
}