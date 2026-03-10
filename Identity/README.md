# Identity

JWT authentication, claims extraction, and shared role names.

## Architecture

Extends ASP.NET Core authentication. Provides `IdentityUser`, `ClaimsPrincipalExtensions`, `SharedRoleNames`, `DefaultPolicies`, and Keycloak JWT configuration.

## Main Abstractions

```csharp
public record IdentityUser(Guid Id, string Name, string Email);

public static class ClaimsPrincipalExtensions
{
    Guid GetUserId(this ClaimsPrincipal? principal);
    IdentityUser GetUser(this ClaimsPrincipal? principal);
    bool HasFullAccess(this ClaimsPrincipal principal);
}

public static class SharedRoleNames
{
    public const string FullAccess = "full-access";
    public const string InternalAccess = "internal-access";
}
```

## Usage Example

```csharp
// In endpoint handler
public static async Task<IResult> GetTab(Guid id, ClaimsPrincipal user, ISender sender)
{
    var userId = user.GetUserId();
    var identityUser = user.GetUser();  // Id, Name, Email from JWT claims

    var query = new GetTabQuery(id, userId);
    var result = await sender.Send(query);
    return result.Ok();
}

// Internal endpoint authorization
.RequireAuthorization(SharedRoleNames.InternalAccess)
```
