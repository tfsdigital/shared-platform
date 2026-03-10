# Results

Result pattern for error handling instead of exceptions.

## Architecture

- **Shared.Results**: `Result`, `Result<T>`, `Error`, `ErrorResult`, `NotFoundResult`
- **Shared.Results.Extensions**: HTTP mapping (`.Ok()`, `.BadRequest()`, `.NotFound()`)

## Main Abstractions

```csharp
public record Error(string Code, string Description);

Result.Success()
Result.Success(response)
Result.Error(error)
Result.Error(errors)
Result.NotFound(error)

Result<T>.Success(data)
Result<T>.Error(error)
Result<T>.NotFound(error)
```

## Usage Example

```csharp
// In handler
if (tab is null)
    return Result.NotFound(TabErrors.NotFound(id));

if (!tab.CanClose)
    return Result.Error(TabErrors.CannotClose);

return Result.Success();

// In endpoint
var result = await sender.Send(command);
return result.Ok();  // 200, 400, or 404 based on result type
```

Extensions map: `Success` → 200/201, `ErrorResult` → 400, `NotFoundResult` → 404.
