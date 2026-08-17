using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestionService.Outbox.Enums;
using QuestionService.Outbox.Messages;

namespace QuestionService.DAL.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.ProcessedAt);
        builder.Property(x => x.ErrorMessage);
        builder.Property(x => x.RetryCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.NextRetryAt);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired().HasDefaultValue(OutboxMessageStatus.Pending);

        var allowedStatuses = string.Join(',', Enum.GetValues<OutboxMessageStatus>().Select(x => (int)x));
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_OutboxMessage_Status_Enum", $"""
                 "{nameof(OutboxMessage.Status)}" IN ({allowedStatuses})
                 """);

            t.HasCheckConstraint("CK_OutboxMessage_ProcessedAt_Status", $"""
                 ("{nameof(OutboxMessage.Status)}" = {(int)OutboxMessageStatus.Processed} AND "{nameof(OutboxMessage.ProcessedAt)}" IS NOT NULL) OR "{nameof(OutboxMessage.Status)}" <> {(int)OutboxMessageStatus.Processed}
                 """);
        });
    }
}