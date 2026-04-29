using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class ShopItemConfiguration : IEntityTypeConfiguration<ShopItem>
{
    public void Configure(EntityTypeBuilder<ShopItem> builder)
    {
        builder.ToTable("shop_items", "gamification");

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.IconUrl).HasMaxLength(500).IsRequired();
        builder.Property(s => s.AssetData).HasColumnType("jsonb");
        builder.Property(s => s.Slot).HasConversion<int>();
        builder.Property(s => s.StackingPolicy).HasConversion<int>();

        builder.HasIndex(s => new { s.TenantId, s.IsActive });
    }
}
