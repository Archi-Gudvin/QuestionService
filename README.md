# Flow OverStack – QuestionService
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_QuestionService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_QuestionService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_QuestionService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_QuestionService)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_QuestionService&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_QuestionService)

## Project Overview

QuestionService is a microservice in the Flow OverStack platform responsible for managing questions, including creation,
editing, retrieval, and moderation. It handles question-related business logic, such as tags, views, and votes.

## 🚀 Quick Start a ready-made API

The entire flow OverStack platform - all five services plus Keycloak, Kafka, Postgres, Redis
and the observability stack - comes up with one command via
[flow-OverStack/Setup](https://github.com/flow-OverStack/Setup), pre-seeded with mock data:

```bash
git clone --recurse-submodules --shallow-submodules https://github.com/flow-OverStack/Setup.git
cd Setup
./setup.sh
```

The [Setup README](https://github.com/flow-OverStack/Setup#readme) covers prerequisites,
flags (`--lite`, `--reseed`, `--migrate`, `--reset`), teardown, and the published endpoints.

To run QuestionService from source instead, see
[Getting Started for developers](#getting-started-for-developers).

## Technologies and Patterns Used

* **.NET 9 & C#** — Core framework and language
* **ASP.NET Core** — HTTP API.
* **Entity Framework Core with PostgreSQL** — Data access (Repository & Unit of Work patterns) to PostgreSQL database
* **Kafka** — Message queue that listens to main events
* **gRPC** — High-performance RPC interface
* **Redis** — Caching layer with short-lived entity caching and negative caching (null values caching)
* **Hot Chocolate** — GraphQL endpoint with built-in support for pagination, filtering, and sorting
* **Clean Architecture** — Layered separation (Domain, Application, Infrastructure, Presentation)
* **Outbox Pattern** — ensures reliable message delivery to the message queue 
* **Decorator Pattern** — allows behavior to be added to individual objects dynamically without affecting others. In this project, it is used to implement caching.
* **Hangfire** — Hosted services for background jobs
* **Resilience** — Standard .NET resilience handler for HTTP clients (retries, circuit breaker, timeout), Hangfire retries and MassTransit retries, circuit breaker and kill switch
* **Observability** — Traces, logs, and metrics collected via OpenTelemetry and Logstash, exported to Aspire dashboard, Jaeger, ElasticSearch, and Prometheus
* **Monitoring & Visualization** — Dashboards in Grafana, Kibana, and Aspire
* **Health Checks** — Status endpoints to monitor service availability and dependencies
* **xUnit & Coverlet** — Automated unit and integration testing with code coverage
* **SonarQube** — Code quality and coverage analysis

## Architecture and Design
This service follows the principles of Clean Architecture. The solution is split into multiple projects that correspond to each architectural layer.

![Clean Architecture](https://www.milanjovanovic.tech/blogs/mnw_017/clean_architecture.png?imwidth=1920)

| Layer              | Project                                                                                                                                             |
|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------|
| **Presentation**   | QuestionService.GraphQl, QuestionService.Api                                                                                                        |
| **Application**    | QuestionService.Application                                                                                                                         |
| **Domain**         | QuestionService.Domain                                                                                                                              |
| **Infrastructure** | QuestionService.BackgroundJobs, QuestionService.Cache, QuestionService.DAL, QuestionService.Grpc, QuestionService.Messaging, QuestionService.Outbox |

Full system design on Miro: [Application Structure Board](https://miro.com/app/board/uXjVLx6YYx4=/?share_link_id=993967197754)

## Getting Started for developers

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://www.docker.com/)

### Installation

1. Clone the repo
2. Reconfigure if needed `appsettings.json` and `.NET User Secrets` in `QuestionService.Api` with your database and Redis.
   `.NET User Secrets` example: 
   ```json
   {
       "ConnectionStrings": {
           "PostgresSQL": "Server=localhost;Port=5435; Database=question-service-db; User Id=<YOUR-USER-ID>; Password=<YOUR-PASSWORD>"
       },
       "RedisSettings": {
           "Password": "<YOUR-PASSWORD>"
       }
   }
   ```
3. Start the [UserService](https://github.com/flow-OverStack/UserService/tree/master?tab=readme-ov-file#getting-started-for-developers) first, as QuestionService depends on it and common services (such as Kafka, Keycloak, etc.)
4. Start dependencies (you can use [Quick Start](#-quick-start-a-ready-made-api) or run your own services)
5. Run the API:

   ```bash
   cd QuestionService.Api
   dotnet run
   ```
   or use your IDE.
## API Documentation

The following endpoints are available by default:

| REST API & Swagger                             | GraphQL Endpoint                | 
|------------------------------------------------|---------------------------------| 
| https://localhost:7067/swagger/v1/swagger.json | 	https://localhost:7067/graphql | 

## Testing

Run unit and functional tests:

```bash
cd QuestionService.Tests
dotnet test --filter Category=Functional
dotnet test --filter Category=Unit
```

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_QuestionService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_QuestionService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_QuestionService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_QuestionService)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_QuestionService&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_QuestionService)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to your branch 
5. Open a Pull Request

Please follow the existing code conventions and include tests for new functionality.
You are also welcome to open issues for bug reports, feature requests, or to discuss improvements. 

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/flow-OverStack/QuestionService/blob/master/LICENSE) file for details.

## Contact

For questions or suggestions open an issue.
