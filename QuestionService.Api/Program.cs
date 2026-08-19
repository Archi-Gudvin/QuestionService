using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using QuestionService.Api;
using QuestionService.Api.Middlewares;
using QuestionService.Api.Settings;
using QuestionService.Application.DependencyInjection;
using QuestionService.Application.Settings;
using QuestionService.BackgroundJobs.DependencyInjection;
using QuestionService.Cache.DependencyInjection;
using QuestionService.Cache.Settings;
using QuestionService.DAL.DependencyInjection;
using QuestionService.GraphQl.DependencyInjection;
using QuestionService.GrpcClient.DependencyInjection;
using QuestionService.GrpcClient.Settings;
using QuestionService.GrpcServer.DependencyInjection;
using QuestionService.Messaging.DependencyInjection;
using QuestionService.Messaging.Settings;
using QuestionService.Outbox.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
builder.Services.Configure<PaginationRules>(builder.Configuration.GetSection(nameof(PaginationRules)));
builder.Services.Configure<GrpcHosts>(builder.Configuration.GetSection(nameof(GrpcHosts)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddLocalization(options => options.ResourcesPath = nameof(QuestionService.Application.Resources));

builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddGraphQl();
builder.Services.AddGrpcClients();
builder.Services.AddGrpcServices();
builder.Services.AddMassTransitServices();
builder.Services.AddOutbox();
builder.Services.AddHangfire(builder.Configuration);

builder.Host.AddLogging(builder.Configuration);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddCache();
builder.Services.AddApplication();

builder.AddOpenTelemetry();
builder.Services.AddHealthChecks(builder.Configuration);
builder.WebHost.ConfigurePorts(builder.Configuration);
builder.Services.AddCors(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseStatusCodePages();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRequestLogging();

app.UseRouting();
app.MapControllers();
app.UseLocalization();
app.UseGraphQl();
app.UseGrpcServices();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfire();
app.SetupHangfireJobs();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseForwardedHeaders(builder.Configuration);
app.UseCors("DefaultCorsPolicy");

app.UseMiddleware<ClaimsValidationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    await app.Services.MigrateDatabaseAsync();
}

app.UseSwagger();

app.LogListeningUrls();

await app.RunAsync();