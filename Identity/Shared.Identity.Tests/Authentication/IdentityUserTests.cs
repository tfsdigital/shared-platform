using Shared.Identity.Authentication;

namespace Shared.Identity.Tests.Authentication;

public class IdentityUserTests
{
    [Fact]
    public void Constructor_WhenCalledWithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var email = "john.doe@example.com";

        // Act
        var user = new IdentityUser(id, name, email);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyGuid_ShouldAcceptEmptyGuid()
    {
        // Arrange
        var id = Guid.Empty;
        var name = "John Doe";
        var email = "john.doe@example.com";

        // Act
        var user = new IdentityUser(id, name, email);

        // Assert
        Assert.Equal(Guid.Empty, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyName_ShouldAcceptEmptyName()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "";
        var email = "john.doe@example.com";

        // Act
        var user = new IdentityUser(id, name, email);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(string.Empty, user.Name);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyEmail_ShouldAcceptEmptyEmail()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var email = "";

        // Act
        var user = new IdentityUser(id, name, email);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void Equals_WhenComparingUsersWithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var email = "john.doe@example.com";
        var user1 = new IdentityUser(id, name, email);
        var user2 = new IdentityUser(id, name, email);

        // Act
        var result = user1.Equals(user2);

        // Assert
        Assert.True(result);
        Assert.Equal(user1, user2);
    }

    [Fact]
    public void Equals_WhenComparingUsersWithDifferentIds_ShouldReturnFalse()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var user1 = new IdentityUser(Guid.NewGuid(), name, email);
        var user2 = new IdentityUser(Guid.NewGuid(), name, email);

        // Act
        var result = user1.Equals(user2);

        // Assert
        Assert.False(result);
        Assert.NotEqual(user1, user2);
    }

    [Fact]
    public void Equals_WhenComparingUsersWithDifferentNames_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "john.doe@example.com";
        var user1 = new IdentityUser(id, "John Doe", email);
        var user2 = new IdentityUser(id, "Jane Doe", email);

        // Act
        var result = user1.Equals(user2);

        // Assert
        Assert.False(result);
        Assert.NotEqual(user1, user2);
    }

    [Fact]
    public void Equals_WhenComparingUsersWithDifferentEmails_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var user1 = new IdentityUser(id, name, "john.doe@example.com");
        var user2 = new IdentityUser(id, name, "jane.doe@example.com");

        // Act
        var result = user1.Equals(user2);

        // Assert
        Assert.False(result);
        Assert.NotEqual(user1, user2);
    }

    [Fact]
    public void GetHashCode_WhenCalledOnUsersWithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var email = "john.doe@example.com";
        var user1 = new IdentityUser(id, name, email);
        var user2 = new IdentityUser(id, name, email);

        // Act
        var hashCode1 = user1.GetHashCode();
        var hashCode2 = user2.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void ToString_WhenCalled_ShouldReturnStringRepresentation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var email = "john.doe@example.com";
        var user = new IdentityUser(id, name, email);

        // Act
        var result = user.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(id.ToString(), result);
        Assert.Contains(name, result);
        Assert.Contains(email, result);
    }
}
