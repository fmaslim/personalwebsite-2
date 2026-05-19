# PersonalWebsite API

A .NET 8 Web API built as a personal learning and reference project. It demonstrates real-world patterns used in production APIs, including service abstraction, structured error handling, pagination, authentication, logging, and performance-focused query design.

---

## Project Overview

This API is backed by the [AdventureWorks](https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure) SQL Server database and exposes endpoints across several domains: products, orders, customers, employees, patients, files, and authentication. It is not a production application — it serves as a sandbox for practising and showcasing common backend engineering patterns.

---

## Tech Stack

| Concern | Technology |
|---------|-----------|
| Framework | ASP.NET Core 8.0 |
| ORM | Entity Framework Core 8 (SQL Server) |
| Logging | Serilog + Azure Application Insights |
| Auth | JWT Bearer (via `Microsoft.AspNetCore.Authentication.JwtBearer`) |
| API Docs | Swagger / Swashbuckle |
| Migrations | EF Core Migrations |

---

## Key Patterns

### ServiceResult Pattern
All service methods return `ServiceResult<T>` — a generic wrapper that carries the result data, a success flag, HTTP status code, and structured errors. Controllers stay thin: they call the service and forward the result to the client via the `.ToActionResult()` extension method.

```csharp
// Service
public async Task<ServiceResult<HelloResponseDto>> GetGreetingAsync(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return ServiceResult<HelloResponseDto>.ValidationFail([new FieldError { Field = "name", Message = "Name is required." }]);

    return ServiceResult<HelloResponseDto>.Ok(new HelloResponseDto { Message = $"Hello, {name}!" });
}

// Controller
var result = await _greetingService.GetGreetingAsync(name);
return result.ToActionResult();
```

Error factory methods on `ServiceResult<T>`:

| Factory | HTTP Status |
|---------|-------------|
| `Ok(data)` | 200 |
| `ValidationFail(fieldErrors)` | 400 |
| `Fail(List<ServiceError>, 400)` | 400 (multi-error) |
| `NotFound(message)` | 404 |
| `Conflict(message)` | 409 |
| `Fail(message, ServiceErrorType.Unexpected)` | 500 |

### Multi-Error Validation
Rather than short-circuiting on the first error, services accumulate all validation failures into a `List<ServiceError>` and return them together. This gives the client a complete picture of what is wrong in a single round-trip.

### Service Abstraction
All services are defined as interfaces under `Services/Abstractions/` and implemented under `Services/Implementations/`. They are registered via DI in `Program.cs` using `AddScoped<IXxx, Xxx>()`, keeping controllers decoupled from implementation details.

### Correlation ID Middleware
Every request is assigned a `X-Correlation-Id` header (generated if not supplied by the caller). It is propagated through the Serilog logging scope so every log entry for a request shares the same ID — making distributed tracing straightforward.

### Global Exception Handling
Unhandled exceptions are caught by `GlobalExceptionMiddleware` and mapped to a consistent `ApiErrorResponse` shape, preventing stack traces leaking to the client.

### Pagination
Endpoints that return collections use `PagedResponse<T>` (with `Items`, `PageNumber`, `PageSize`, `TotalRecords`, `TotalPages`) or the PRD-shaped `PagedResponseDto<T>` (with `Data`, `TotalCount`). Both expose `HasNextPage` / `HasPreviousPage` for cursor navigation.

---

## Project Structure

```
PersonalWebsite.Api/
├── Controllers/          # Thin API controllers; each calls a service and returns ToActionResult()
│   └── Files/            # Development reference files (PRDs, extraction scripts)
├── DTOs/                 # Request/response data transfer objects
│   └── Common/           # Shared types: ServiceResult<T>, PagedResponse<T>, ApiErrorResponse
├── Extensions/           # Extension methods (ToActionResult, paging, query helpers)
├── Middleware/           # CorrelationId, GlobalExceptionMiddleware
├── Migrations/           # EF Core migration history
├── Models/               # EF Core entity models + AdventureWorksContext
├── Services/
│   ├── Abstractions/     # Service interfaces
│   ├── Implementations/  # Service classes
│   └── PerformanceTraining/ # Query performance demos (accounts, customers, orders, patients)
└── Program.cs            # DI registration, middleware pipeline, Swagger, JWT config
```

---

## Running Locally

1. Set the SQL Server connection string in `appsettings.Development.json`.
2. If Azure Application Insights is not configured, supply a dummy key to suppress startup errors:
   ```powershell
   $env:ApplicationInsights__ConnectionString = 'InstrumentationKey=00000000-0000-0000-0000-000000000000'
   dotnet run
   ```
3. Swagger UI is available at `http://localhost:5000/swagger`.

---

## Reference Files

PRDs and extraction scripts used during feature development live in [`Controllers/Files/`](Controllers/Files/).
