using Microsoft.EntityFrameworkCore;

namespace JGUZDV.Outbox.EntityFrameworkCore;

internal class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("Messages", "Outbox");

        builder.Property(m => m.MessageType).IsRequired();
        builder.Property(m => m.MessageData).IsRequired();
        builder.Property(m => m.DueDate).IsRequired();
        
        builder.HasKey(m => m.Id);
        builder.HasIndex(m => m.MessageType);

        builder.HasIndex(m => m.DueDate).IsDescending();
        builder.HasIndex(m => m.ProcessedDate).IsDescending();
        builder.HasIndex(m => m.Tags);

        builder.ComplexCollection(m => m.Failures, c => c.ToJson());
    }
}
