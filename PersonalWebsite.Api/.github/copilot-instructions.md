# Copilot Instructions — PersonalWebsite API

## Project Overview
This is a .NET 8 Web API personal learning project backed by an AdventureWorks SQL Server database. It is a sandbox for practising and demonstrating real-world backend patterns. The codebase uses EF Core 8, Serilog, JWT Bearer auth, and Swashbuckle/Swagger.

---

## Architecture

- **Controllers** live in `Controllers/`. They are thin — one service call, one `return result.ToActionResult()`. No business logic.
- **Services** follow the interface/implementation split:
  - Interfaces → `Services/Abstractions/IXxx.cs`
  - Implementations → `Services/Implementations/XxxService.cs`
  - Register in `Program.cs` as `builder.Services.AddScoped<IXxx, XxxService>()`
- **DTOs** live in `DTOs/`. Shared/common types (ServiceResult, PagedResponse, ApiErrorResponse) live in `DTOs/Common/`.
- **Models** (EF Core entities) live in `Models/`. The DbContext is `AdventureWorksContext`.
- **Middleware** lives in `Middleware/`. Currently: `CorrelationIdMiddleware`, `GlobalExceptionMiddleware`.
- **Extensions** live in `Extensions/`. Key one: `ServiceResultExtensions.ToActionResult<T>()`.

---

## ServiceResult Pattern

All service methods **must** return `ServiceResult<T>`. Never return raw data or throw exceptions for expected error cases.

```csharp
// Service method signature
public async Task<ServiceResult<MyDto>> DoSomethingAsync(...)

// Controller (always exactly this shape)
var result = await _service.DoSomethingAsync(...);
return result.ToActionResult();
```

### Factory methods on `ServiceResult<T>`

| Factory | Use when | HTTP status |
|---------|----------|-------------|
| `Ok(data)` | Success with data | 200 |
| `ValidationFail(List<FieldError>)` | Single-field validation errors | 400 |
| `Fail(List<ServiceError>, 400)` | Multi-param validation (accumulate all errors) | 400 |
| `NotFound(message)` | Resource not found | 404 |
| `Conflict(message)` | Duplicate / state conflict | 409 |
| `Fail(message, ServiceErrorType.Unexpected)` | Unexpected server error | 500 |

`ToActionResult()` in `Extensions/ServiceResultExtensions.cs` maps `ServiceErrorType` → the correct `IActionResult` automatically.

---

## Validation Rules

- **Never short-circuit** on the first error. Always collect ALL validation errors before returning.
- Use `ValidationFail(List<FieldError>)` when errors are tied to specific fields (`Field`, `Message`).
- Use `Fail(List<ServiceError>, 400)` with `ServiceErrorType = Validation` on each `ServiceError` when validating multiple independent query parameters.
- Validation belongs in the **service layer**, not the controller.

```csharp
// Multi-error accumulation pattern
var errors = new List<ServiceError>();

if (invalidConditionA)
    errors.Add(new ServiceError { Code = "Validation", Message = "...", Type = ServiceErrorType.Validation });

if (invalidConditionB)
    errors.Add(new ServiceError { Code = "Validation", Message = "...", Type = ServiceErrorType.Validation });

if (errors.Count > 0)
{
    var failure = ServiceResult<T>.Fail(errors, 400);
    failure.ServiceErrorType = ServiceErrorType.Validation;
    return failure;
}
```

---

## Logging

- Inject `ILogger<T>` via the constructor.
- Never use `Console.WriteLine`.
- Serilog is configured in `Program.cs` via `Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(...)`.
- Every request gets a `X-Correlation-Id` header via `CorrelationIdMiddleware` — it appears in all log entries for that request automatically.

---

## Pagination

Two paged response shapes exist — use the right one for the context:

| Type | Properties | Use for |
|------|-----------|---------|
| `PagedResponse<T>` (`DTOs/Common/`) | `Items`, `PageNumber`, `PageSize`, `TotalRecords`, `TotalPages` | Existing EF-backed endpoints |
| `PagedResponseDto<T>` (`DTOs/Common/`) | `Data`, `PageNumber`, `PageSize`, `TotalCount`, `TotalPages` | Hello World / PRD-specified endpoints |

---

## Coding Conventions

- Use `var` for local variables.
- Use file-scoped namespaces (`namespace Foo.Bar;`).
- Use `async`/`await` throughout — never `.Result` or `.Wait()`.
- Add `await Task.Yield()` in service methods that are logically async but have no actual I/O, to preserve the async contract.
- Do not add comments unless explicitly asked.
- Do not add try/catch unless handling a specific, expected exception.
- Do not create abstractions or helper classes for one-off operations.
- Do not refactor or improve code that was not part of the request.

---

## Running Locally

SQL Server connection string goes in `appsettings.Development.json`. If Azure Application Insights is not set up, use a dummy key to suppress startup errors:

```powershell
$env:ApplicationInsights__ConnectionString = 'InstrumentationKey=00000000-0000-0000-0000-000000000000'
dotnet run
```

Swagger UI: `http://localhost:5000/swagger`
