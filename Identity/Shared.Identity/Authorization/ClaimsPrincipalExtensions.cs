using System.Security.Claims;
using Shared.Identity.Authentication;

namespace Shared.Identity.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new InvalidOperationException("User identifier is unavailable");
    }

    public static IdentityUser GetUser(this ClaimsPrincipal? principal)
    {
        string? stringUserId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? name = principal?.FindFirst("name")?.Value;
        string? email = principal?.FindFirst(ClaimTypes.Email)?.Value;

        if (!Guid.TryParse(stringUserId, out var userId))
            throw new InvalidOperationException("User identifier is unavailable");

        if (name is null)
            throw new InvalidOperationException("User name is unavailable");

        if (email is null)
            throw new InvalidOperationException("User email is unavailable");

        return new IdentityUser(userId, name, email);
    }

    public static bool HasFullAccess(this ClaimsPrincipal principal)
    {
        return principal.IsInRole(SharedRoleNames.FullAccess);
    }
}
