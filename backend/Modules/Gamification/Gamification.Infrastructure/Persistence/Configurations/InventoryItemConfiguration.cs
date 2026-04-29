using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items", "gamification");

        builder.Property(i => i.Source).HasConversion<int>();

        builder.HasIndex(i => new { i.ProfileId, i.ShopItemId }).IsUnique();
        builder.HasIndex(i => i.ProfileId);
    }
}
