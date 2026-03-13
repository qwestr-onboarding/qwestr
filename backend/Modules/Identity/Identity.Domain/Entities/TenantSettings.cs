using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Identity.Domain.Entities;

/// <summary>
/// Настройки компании
/// </summary>
public sealed class TenantSettings : Entity<Guid>, IAuditableEntity
{
    private TenantSettings() { }

    public Guid TenantId { get; set; }

    [MaxLength(2048)]
    public string? CompanyLogoUrl { get; set; }

    [Required]
    [MaxLength(64)]
    public string Timezone { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string Language { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant Tenant { get; set; } = null!;

    public static TenantSettings Create(Guid tenantId)
    {
        return new TenantSettings
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Timezone = "UTC",
            Language = "ru",
            CreatedAt = DateTime.UtcNow
        };
    }
}
