using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qwestr.Modules.Identity.Domain.Enums;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Domain.Entities;

/// <summary>
/// Приглашение в компанию и на регистрацию
/// </summary>
public sealed class Invitation : AggregateRoot<Guid>
{
    private Invitation() { }

    public Guid TenantId { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = null!;

    public UserRole Role { get; set; }
    public Guid InvitedByUserId { get; set; }

    [Required]
    [MaxLength(512)]
    public string Token { get; set; } = null!;

    public InvitationStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant Tenant { get; set; } = null!;

    [ForeignKey(nameof(InvitedByUserId))]
    public User InvitedByUser { get; set; } = null!;

    public static Invitation Create(
        Guid tenantId,
        string email,
        UserRole role,
        Guid invitedByUserId,
        string token,
        DateTime expiresAt)
    {
        return new Invitation
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            Role = role,
            InvitedByUserId = invitedByUserId,
            Token = token,
            Status = InvitationStatus.Pending,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Accept()
    {
        Status = InvitationStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = InvitationStatus.Cancelled;
    }

    public void Expire()
    {
        Status = InvitationStatus.Expired;
    }
}
