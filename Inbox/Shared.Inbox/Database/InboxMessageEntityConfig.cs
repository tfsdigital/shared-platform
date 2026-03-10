using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Inbox.Models;

namespace Shared.Inbox.Database;

public class InboxMessageEntityConfig : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("InboxMessages").HasKey(p => p.Id);

        builder.Property(o => o.Id).ValueGeneratedNever();
        builder.Property(o => o.Type).HasMaxLength(500);
        builder.Property(o => o.Headers).HasColumnType("jsonb");
        builder.Property(o => o.Content).HasColumnType("jsonb");

        builder.HasIndex(o => o.OccurredOn);
        builder.HasIndex(o => o.ProcessedOn);
        builder.HasIndex(o => o.ErrorHandledOn);
    }
}
