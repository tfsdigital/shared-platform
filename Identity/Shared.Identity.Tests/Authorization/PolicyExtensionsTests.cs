using Microsoft.AspNetCore.Authorization;
using Shared.Identity.Authorization;

namespace Shared.Identity.Tests.Authorization;

public class PolicyExtensionsTests
{
    [Fact]
    public void RequireFullAccess_AddsFullAccessRoleRequirement()
    {
        // Arrange
        var builder = new AuthorizationPolicyBuilder();

        // Act
        var result = builder.RequireFullAccess();

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result);
    }

    [Fact]
    public void RequireFullAccess_WithExistingRequirements_PreservesRequirements()
    {
        // Arrange
        var builder = new AuthorizationPolicyBuilder();
        builder.RequireAuthenticatedUser();

        // Act
        var result = builder.RequireFullAccess();

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result);
    }
}
