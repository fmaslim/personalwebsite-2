---
description: "Add a new API endpoint following the ServiceResult pattern. Use when: creating a new endpoint, adding a route, scaffolding a controller action with service and DTO."
name: "New Endpoint"
argument-hint: "Describe the endpoint (e.g. GET /api/orders/{id} returns order details)"
agent: "agent"
---

Scaffold a new API endpoint for this project following the conventions in [copilot-instructions.md](../copilot-instructions.md).

Before writing any code, ask me these questions (all at once, not one at a time):

1. **Route** — What is the HTTP verb and route? (e.g. `GET /api/orders/{id}`)
2. **Purpose** — What does it do in plain English?
3. **Response DTO** — What fields should the response contain? Does a suitable DTO already exist?
4. **Request inputs** — Any route params, query params, or request body fields?
5. **Validation rules** — What is invalid? Which fields can each fail independently (multi-error) vs. together (single field error)?
6. **Data source** — Is this EF Core (existing DbContext), in-memory, or calling another service?
7. **Auth** — Does this endpoint require JWT authentication?

Once I answer, implement the following files (only create files that do not already exist):

- `DTOs/<Name>Dto.cs` — response DTO (skip if reusing an existing one)
- `Services/Abstractions/IXxxService.cs` — add the method signature (or create the interface if it doesn't exist)
- `Services/Implementations/XxxService.cs` — implement the method:
  
  - Use `async`/`await`; 
  - Use async/await only when there is real I/O such as EF Core calls.
  - Do not fake async with Task.Yield().
  - Validate inputs in the service layer, never in the controller
  - Accumulate ALL validation errors before returning — never short-circuit
  - Use `ValidationFail(List<FieldError>)` for single-field errors
  - Use `Fail(List<ServiceError>, 400)` + `ServiceErrorType = Validation` for multi-param errors
  - Return `ServiceResult<T>.Ok(data)` on success
- `Controllers/XxxController.cs` — add the action:
  - One line: call the service, one line: `return result.ToActionResult()`
  - Add `[ProducesResponseType]` attributes for 200 and any error codes

After implementing, confirm with a `dotnet build` to verify it compiles.
