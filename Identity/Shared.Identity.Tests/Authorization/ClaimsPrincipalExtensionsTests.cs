using System.Security.Claims;
using Shared.Identity.Authorization;

namespace Shared.Identity.Tests.Authorization;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_WithValidUserId_ReturnsGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_WithInvalidUserId_ThrowsInvalidOperationException()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => principal.GetUserId());
    }

    [Fact]
    public void GetUserId_WithNullPrincipal_ThrowsInvalidOperationException()
    {
        // Arrange
        ClaimsPrincipal? principal = null;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => principal.GetUserId());
    }

    [Fact]
    public void GetUser_WithValidClaims_ReturnsIdentityUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "John Doe";
        var email = "john@example.com";
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("name", name),
            new Claim(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetUser();

        // Assert
        Assert.Equal(userId, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public void GetUser_WithMissingName_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "john@example.com";
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => principal.GetUser());
    }

    [Fact]
    public void HasFullAccess_WithFullAccessRole_ReturnsTrue()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, SharedRoleNames.FullAccess)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.HasFullAccess();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFullAccess_WithoutFullAccessRole_ReturnsFalse()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "other-role")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.HasFullAccess();

        // Assert
        Assert.False(result);
    }
}
