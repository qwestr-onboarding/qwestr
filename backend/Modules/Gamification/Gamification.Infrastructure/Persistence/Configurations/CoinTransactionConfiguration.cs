using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class CoinTransactionConfiguration : IEntityTypeConfiguration<CoinTransaction>
{
    public void Configure(EntityTypeBuilder<CoinTransaction> builder)
    {
        builder.ToTable("coin_transactions", "gamification");

        builder.Property(t => t.Source).HasConversion<int>();
        builder.Property(t => t.Reason).HasMaxLength(500);

        builder.HasIndex(t => new { t.TenantId, t.ProfileId, t.OccurredAt });
    }
}
