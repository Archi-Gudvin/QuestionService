using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;
using QuestionService.Api.Settings;
using QuestionService.Cache.Settings;
using QuestionService.DAL;
using QuestionService.GrpcClient;
using QuestionService.GrpcClient.Settings;
using QuestionService.Outbox.Events;
using QuestionService.Tests.FunctionalTests.Configurations;
using QuestionService.Tests.FunctionalTests.Configurations.TestServices;
using QuestionService.Tests.FunctionalTests.Extensions;
using QuestionService.Tests.FunctionalTests.Helper;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using WireMock.Server;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _questionServicePostgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithDatabase("question-service-db")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7")
        .Build();

    private WireMockServer _wireMockServer = null!;

    public async Task InitializeAsync()
    {
        await _questionServicePostgreSql.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _questionServicePostgreSql.StopAsync();
        await _redisContainer.StopAsync();
        _wireMockServer.StopServer();
        _wireMockServer.Dispose();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:PostgresSQL", _questionServicePostgreSql.GetConnectionString() }
                }!)
                .Build();

            config.AddConfiguration(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            var connectionString = _questionServicePostgreSql.GetConnectionString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateAsyncScope();
            scope.PrepPopulation();

            _wireMockServer = _wireMockServer.StartIdentityServer();

            services.RemoveAll<IOptions<KeycloakSettings>>();
            services.Configure<KeycloakSettings>(x =>
            {
                x.Host = _wireMockServer.Url!;
                x.Realm = WireMockIdentityServerExtensions.RealmName;
                x.Audience = TokenHelper.GetAudience();
            });

            services.RemoveAll<IOptions<GrpcHosts>>();
            services.Configure<GrpcHosts>(x => { x.UsersHost = "TestUsersHost"; });

            services.RemoveAll<UserService.UserServiceClient>();
            services.AddSingleton<UserService.UserServiceClient, GrpcTestUserService>();

            services.RemoveAll<ITopicProducer<BaseEvent>>();
            services.AddScoped<ITopicProducer<BaseEvent>>(_ =>
            {
                var testProducer = new Mock<ITopicProducer<BaseEvent>>();
                return testProducer.Object;
            });

            services.RemoveAll<IOptions<RedisSettings>>();
            services.Configure<RedisSettings>(x =>
            {
                _redisContainer.GetConnectionString().ParseConnectionString(out var host, out var port);
                x.Host = host;
                x.Port = port;
                x.Password = null!;
            });
        });
    }
}