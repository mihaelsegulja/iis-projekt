# Project: IIS Notion Search

## Overview

- **Backend:** ASP.NET Core 8 Web API (Clean Architecture)
- **Database:** SQL Server (Docker)
- **Frontend:** Angular 21
- **Key Features:** JWT Auth (Access/Refresh), Notion API Integration, XML/JSON Validation, SOAP, GraphQL

## Quick Start

### Prerequisites

- .NET 8 SDK installed
- Docker & Docker Compose installed
- Node.js ^20.19.0 || ^22.12.0 || ^24.0.0
- **HTTPS Trust:** Required for gRPC and local development:
  ```bash
  dotnet dev-certs https --trust
  ```
- (Optional) dotnet-ef tools if you need to run migrations: `dotnet tool install --global dotnet-ef`

### Setup

1. Start SQL Server with Docker Compose

- From the repository root run:

```bash
docker-compose up -d
```

- This starts a SQL Server container named `iis-notionsearch-db` and exposes port 1433 on localhost.
- Default credentials (from docker-compose.yml):
  - SA user: `sa`
  - Password: `YourStrong!Passw0rd`

2. Run the API

- From the repository root run:

```bash
dotnet run --project IISNotionSearch/IISNotionSearch.API/IISNotionSearch.API.csproj
```

- API is available at: <https://localhost:7026>. Swagger UI is available on `/swagger` in Development.

3. Run the frontend

- From the repository root run:

```bash
cd iis-notionsearch-fe

npm install

npm run start
```

- Frontend is available at: <http://localhost:4200>

### Database migrations (if needed)

- The project uses Entity Framework Core (Microsoft.EntityFrameworkCore.SqlServer) and applies migrations on start of the application.
- If you need to create or apply migrations (from `IISNotionSearch` solution dir):

**create a migration**

```bash
dotnet ef migrations add SomeMigrationName --project IISNotionSearch.Repository --startup-project IISNotionSearch.API
```

**apply migrations**

```bash
dotnet ef database update --project IISNotionSearch.Repository --startup-project IISNotionSearch.API
```

### Stopping and Cleanup

- To stop the database container:

```bash
docker-compose down
```

- Data is persisted to a named Docker volume `sqlserverdata` (defined in docker-compose.yml). To remove the volume as well:

```bash
docker-compose down -v
```
