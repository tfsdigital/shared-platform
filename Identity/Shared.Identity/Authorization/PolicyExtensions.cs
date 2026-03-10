using Microsoft.AspNetCore.Authorization;

namespace Shared.Identity.Authorization;

public static class PolicyExtensions
{
    public static AuthorizationPolicyBuilder RequireFullAccess(this AuthorizationPolicyBuilder builder) =>
        builder.RequireRole(SharedRoleNames.FullAccess);
}
