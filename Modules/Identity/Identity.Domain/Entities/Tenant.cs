using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Identity.Domain.Enums;
using Shared.Kernel.Entities;
using Shared.Kernel.Interfaces;

namespace Identity.Domain.Entities;

/// <summary>
/// Компания, пользующаяся сервисом
/// </summary>
public sealed class Tenant : AggregateRoot<Guid>, IAuditableEntity
{
    private Tenant() { }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(63)]
    public string? Subdomain { get; set; }

    public TenantPlan Plan { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Invitation> Invitations { get; set; } = [];
    public TenantSettings? Settings { get; set; }

    public static Tenant Create(string name, string? subdomain = null)
    {
        return new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name,
            Subdomain = subdomain,
            Plan = TenantPlan.Free,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
