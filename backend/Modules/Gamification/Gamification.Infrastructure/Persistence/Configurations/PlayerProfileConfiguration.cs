using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class PlayerProfileConfiguration : IEntityTypeConfiguration<PlayerProfile>
{
    public void Configure(EntityTypeBuilder<PlayerProfile> builder)
    {
        builder.ToTable("player_profiles", "gamification");

        builder.Property(p => p.Status).HasMaxLength(500);

        builder.OwnsOne(p => p.Equipment, eq =>
        {
            eq.Property(e => e.ProfileColorItemId).HasColumnName("equipment_profile_color_item_id");
            eq.Property(e => e.AvatarFrameItemId).HasColumnName("equipment_avatar_frame_item_id");
            eq.Property(e => e.BackgroundItemId).HasColumnName("equipment_background_item_id");
            eq.Property(e => e.PetItemId).HasColumnName("equipment_pet_item_id");
            eq.Property(e => e.ClassItemId).HasColumnName("equipment_class_item_id");
            eq.Property(e => e.WeaponItemId).HasColumnName("equipment_weapon_item_id");
        });

        builder.HasIndex(p => new { p.TenantId, p.UserId }).IsUnique();
        builder.HasIndex(p => new { p.TenantId, p.TotalXp });
    }
}
