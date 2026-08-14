# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

QuestionService is a microservice in the **Flow OverStack** platform (a StackOverflow-style Q&A system). It owns questions and everything attached to them: tags, votes, vote types, and views. It exposes the same domain over **REST (Swagger)**, **GraphQL (HotChocolate + Apollo Federation)**, and **gRPC**, and publishes domain events to **Kafka** via an outbox. It depends on a separate **UserService** (reached over gRPC) and shares common infra (Kafka, Keycloak) with it.

## Build / run / test

Targets **.NET 10** (`global.json` pins SDK `10.0.0`; all projects are `net10.0`). The README still says ".NET 9" — that text is stale; trust `global.json` and the `.csproj` files.

```bash
# Build whole solution
dotnet build

# Run the API (REST + GraphQL + gRPC are all hosted here)
cd QuestionService.Api
dotnet run                      # http://localhost:5170
dotnet run --launch-profile https   # https://localhost:7067

# Swagger:  /swagger/v1/swagger.json     GraphQL: /graphql      Voyager (dev): /graphql-voyager
```

Tests use **xUnit**, **Moq** + **MockQueryable** (unit), **Testcontainers** (Postgres/Redis) and **WireMock.Net** (functional). Tests are tagged with `[Trait("Category", "Unit")]` or `[Trait("Category", "Functional")]`.

```bash
dotnet test --filter Category=Unit
dotnet test --filter Category=Functional

# Single test class or method (substring match on the fully-qualified name):
dotnet test --filter "FullyQualifiedName~QuestionService.Tests.FunctionalTests.Tests.ApiTests"
dotnet test --filter "FullyQualifiedName~ApiTests.RequestForbiddenResource_ShouldBe_Forbidden"
```

**Functional tests need Docker running** — `FunctionalTestWebAppFactory` (in `QuestionService.Tests/FunctionalTests/Base/`) spins up real Postgres + Redis containers, mocks Keycloak with WireMock, mocks the UserService gRPC client (`GrpcTestUserService`), and mocks Kafka via `Mock<ITopicProducer<BaseEvent>>`. Data is seeded by `PrepDb.PrepPopulation()`.

Local dev DB credentials live in **.NET User Secrets** (`ConnectionStrings:PostgresSQL`, `RedisSettings:Password`), not in `appsettings.json`. Docker secrets come from a `.env` file (`QUESTION_DB_PASSWORD`, `REDIS_PASSWORD`).

## Architecture

Clean Architecture, ~12 projects across four layers. Dependencies point inward toward Domain.

- **Presentation** — `QuestionService.Api` (controllers, middleware, DI composition root, Program.cs/Startup.cs), `QuestionService.GraphQl`, `QuestionService.GrpcServer`.
- **Application** — `QuestionService.Application`: business-logic services (`Services/*.cs`), FluentValidation validators, AutoMapper profiles, and **cache decorators** under `Services/Cache/`.
- **Domain** — `QuestionService.Domain`: entities, service interfaces, enums, and the `BaseResult<T>` result type. No outward dependencies.
- **Infrastructure** — `QuestionService.DAL` (EF Core + Postgres), `QuestionService.Messaging` (MassTransit + Kafka), `QuestionService.Outbox`, `QuestionService.Cache` (Redis), `QuestionService.BackgroundJobs` (Hangfire), `QuestionService.GrpcClient` (outbound to UserService).

Key cross-cutting conventions worth knowing before editing:

- **Services return `BaseResult<T>`** (`Domain/Results/`) — `IsSuccess` + `Data` + `ErrorMessage`/`ErrorCode`, not exceptions, for expected failures. Controllers translate this; GraphQL sanitizes via `PublicErrorFilter`.
- **DI is convention-driven.** Services are auto-registered by **Scrutor** assembly scanning (don't expect a hand-written `AddScoped` per service). Read decorators are layered on with `services.Decorate<IGetXService, CacheGetXService>()` — adding a cache layer means writing a decorator, not editing the inner service.
- **Each layer owns an `AddXxx()` extension** in its `DependencyInjection/` folder; `Program.cs` calls them in order (`AddGraphQl`, `AddMassTransitServices`, `AddOutbox`, `AddDataAccessLayer`, `AddApplication`, …). Wire new infra through these, not inline in Program.cs.

### Data layer

`QuestionService.DAL/ApplicationDbContext.cs` (Npgsql). Entity config is fluent, one class per entity in `DAL/Configurations/`, applied via `ApplyConfigurationsFromAssembly`. A `DateInterceptor` auto-stamps `CreatedAt`/`LastModifiedAt` on `IAuditable` entities. Persistence goes through a **generic `BaseRepository<T>`** + **`UnitOfWork`** (`DAL/Repositories/`); multi-step writes use `unitOfWork.BeginTransactionAsync()`. Migrations live in `DAL/Migrations/` — generate with `dotnet ef migrations add <Name> --project QuestionService.DAL --startup-project QuestionService.Api`.

Core entities: **Question** (1→* Tags via `QuestionTag` join, 1→* Votes, 1→* Views), **Tag**, **Vote** (composite key `UserId`+`QuestionId`+`VoteTypeId`, no `Id`), **VoteType** (carries `MinReputationToVote`/`ReputationChange`), **View**.

### Outbox → Kafka (the important async path)

When a domain action happens (ask/delete/upvote/downvote), the service does **not** publish to Kafka directly. Instead, **within the same DB transaction**, a `BaseEvent` is serialized into an `OutboxMessage` row (`Outbox/Services/OutboxService.cs` → repository). This guarantees the event is persisted atomically with the state change.

`OutboxBackgroundService` (a `HostedService`) polls every ~15s, batch 50. For each pending/failed-but-due row, `OutboxProcessor` reflects the stored `.Type`, deserializes `.Content`, resolves the matching `ITopicProducer` via `TopicProducerResolver`, and publishes to Kafka through MassTransit. Status machine: `Pending → Processed` on success; on failure, exponential backoff retries (`5s,10s,15s,30s,1m,5m,10m,1h,12h,24h`), then `Dead` after 10 attempts.

If you add a new event type, it must be discoverable by `Assembly.GetType(message.Type)` and have an `ITopicProducer` that `CanProduce` it. **In functional tests `ITopicProducer<BaseEvent>` is mocked** — assert against the mock, don't expect a real Kafka topic.

### GraphQL

HotChocolate 15 in `QuestionService.GraphQl`. Root queries in `Queries.cs`; object types and their resolvers in `Types/*.cs`. **N+1 is avoided with DataLoaders** in `DataLoaders/` — when adding a relationship field, add/extend a DataLoader rather than querying per-item. Questions/Tags use cursor pagination; Votes/Views use offset pagination (page size capped by `PaginationRules`). Apollo Federation is enabled (`Federation23`, `[Key]` attributes) so types are composed with other services' schemas.

## Conventions

- Nullable reference types and implicit usings are **on** everywhere; file-scoped namespaces throughout.
- Test naming: `MethodName_ShouldBe_Expectation`. Private fields: `_camelCase`.
- Unit tests build their subjects via factory classes in `Tests/UnitTests/Factories/`; mock repos/loggers/mappers come from `Tests/UnitTests/Configurations/`.
- No `.editorconfig`/`.DotSettings` enforced in-repo; quality gates run in CI via **SonarQube** (`.github/workflows/`). CI runs Unit and Functional categories separately with Coverlet coverage.
