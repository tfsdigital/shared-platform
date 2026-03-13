using Microsoft.EntityFrameworkCore;
using Shared.Inbox.Database;
using Shared.Inbox.Models;

namespace Shared.Inbox.Tests.Database;

public class InboxMessageEntityConfigTests
{
    [Fact]
    public void Configure_WhenBuilderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var config = new InboxMessageEntityConfig();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => config.Configure(null!));
    }

    [Fact]
    public void Configure_ShouldMapToInboxMessagesTable()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var config = new InboxMessageEntityConfig();

        // Act
        modelBuilder.ApplyConfiguration(config);

        // Assert
        var entity = modelBuilder.Model.FindEntityType(typeof(InboxMessage));
        Assert.NotNull(entity);
        Assert.Equal("InboxMessages", entity.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetIdAsPrimaryKey()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var config = new InboxMessageEntityConfig();

        // Act
        modelBuilder.ApplyConfiguration(config);

        // Assert
        var entity = modelBuilder.Model.FindEntityType(typeof(InboxMessage));
        var primaryKey = entity!.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties, p => p.Name == nameof(InboxMessage.Id));
    }

    [Fact]
    public void Configure_ShouldSetTypeMaxLength()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var config = new InboxMessageEntityConfig();

        // Act
        modelBuilder.ApplyConfiguration(config);

        // Assert
        var entity = modelBuilder.Model.FindEntityType(typeof(InboxMessage));
        var typeProperty = entity!.FindProperty(nameof(InboxMessage.Type));
        Assert.NotNull(typeProperty);
        Assert.Equal(500, typeProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_ShouldSetJsonbColumnTypeForHeadersAndContent()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var config = new InboxMessageEntityConfig();

        // Act
        modelBuilder.ApplyConfiguration(config);

        // Assert
        var entity = modelBuilder.Model.FindEntityType(typeof(InboxMessage));
        var headersProperty = entity!.FindProperty(nameof(InboxMessage.Headers));
        var contentProperty = entity.FindProperty(nameof(InboxMessage.Content));
        Assert.NotNull(headersProperty);
        Assert.NotNull(contentProperty);
        Assert.Equal("jsonb", headersProperty.GetColumnType());
        Assert.Equal("jsonb", contentProperty.GetColumnType());
    }
}
