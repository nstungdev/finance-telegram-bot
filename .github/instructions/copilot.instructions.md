---
description: Load these instructions when generating, editing, or reviewing C# backend code in this repository.
applyTo: '**/*.cs'
---

# IFD Metrics Coding Instructions

Use these rules for all code changes, reviews, and suggestions.

## Primary Goal

Always prioritize reliability and fast cold starts on serverless deployments with Native AOT compatibility.

## AOT and Trimming Rules

- Avoid reflection-heavy patterns unless there is no alternative.
- Prefer compile-time approaches: source generators, explicit mappings, strongly typed models.
- Prefer `System.Text.Json` source generation via `JsonSerializerContext` for request/response DTOs.
- Do not introduce dynamic runtime code generation (`System.Reflection.Emit`, dynamic proxy libraries).
- Keep serialization contracts explicit and stable (avoid ambiguous converters when possible).

## Performance and Serverless Rules

- Keep per-request allocations low; avoid unnecessary materialization of large collections.
- Use `IAsyncEnumerable<T>` for paged/streaming data flows when applicable.
- Propagate `CancellationToken` from endpoint to service, HTTP calls, and DB operations.
- Use async I/O end-to-end; avoid blocking calls (`.Result`, `.Wait()`, sync-over-async).
- Use bulk database operations for large batches (e.g., PostgreSQL `COPY` + upsert pattern).

## HTTP and External API Rules

- Configure resilient HTTP clients through DI; do not create ad-hoc `HttpClient` instances.
- Always use absolute HTTPS base URIs.
- Handle transient failures and rate limits (`429`, `5xx`) with retry/backoff.
- Validate external identifiers from route/query before using them in downstream calls.

## Data Access Rules

- Use parameterized SQL only; never build SQL from user input.
- Keep SQL explicit and readable; prefer deterministic upsert keys.
- Wrap multi-step writes in transactions.

## Code Quality Rules

- Keep methods focused and easy to test.
- Add short, meaningful logs for operational visibility without leaking secrets.
- Do not log credentials, tokens, or connection strings.
- Preserve existing architecture boundaries (`API` -> `Core` -> `Infrastructure`).

## Logging Rules

### Where to log
- Log only in the **Service layer** — services own the business context.
- Do not log in repositories or HTTP clients; they should only throw exceptions.
- Do not log in API endpoints; `UseSerilogRequestLogging()` in `Program.cs` already covers all HTTP request/response logging.

### Log levels
- `LogInformation` — operational milestones: start and end of a job/sync operation, with a summary result (e.g., total count).
- `LogDebug` — intermediate details useful when debugging (e.g., page counts, intermediate state). Not visible in Production by default.
- `LogWarning` — recoverable/partial failures where the overall operation continues (always include the exception: `LogWarning(ex, ...)`).
- `LogError` — unrecoverable failures that abort the operation (always include the exception).
- `LogCritical` — reserved for application-level failures (data integrity loss, crash imminent).

### Structured logging
- Always use **named placeholders** — never string interpolation — so Serilog captures them as searchable properties:
  ```csharp
  // ✅ correct
  logger.LogInformation("Synced {Count} tickets for {ProjectKey}", count, projectKey);
  // ❌ wrong — breaks structured logging
  logger.LogInformation($"Synced {count} tickets for {projectKey}");
  ```
- Placeholder names should be PascalCase and describe the value's meaning.
- Never include secrets, tokens, or connection strings in any log message or placeholder.

## Review Checklist

When reviewing changes, check these first:

1. Any AOT/trimming risk introduced?
2. Any potential cold-start/per-request allocation regression?
3. Is `CancellationToken` correctly propagated?
4. Are external calls resilient and validated?
5. Are DB writes safe, parameterized, and idempotent where needed?