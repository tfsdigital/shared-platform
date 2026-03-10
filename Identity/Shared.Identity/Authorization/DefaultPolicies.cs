using Microsoft.AspNetCore.Authorization;

namespace Shared.Identity.Authorization;

public static class DefaultPolicies
{
    public static AuthorizationPolicyBuilder CreateOperationalUserPolicy() =>
        new AuthorizationPolicyBuilder(AuthenticationSchemes.Public)
        .RequireAuthenticatedUser()
        .RequireClaim("email_verified", "true");
}
