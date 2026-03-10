using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Identity.Domain.Enums;
using Shared.Kernel.Entities;
using Shared.Kernel.Interfaces;

namespace Identity.Domain.Entities;

/// <summary>
/// Пользователь внутри компании
/// </summary>
public sealed class User : AggregateRoot<Guid>, IAuditableEntity
{
    private User() { }

    public Guid TenantId { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    public UserRole Role { get; set; }
    public bool IsActive { get; set; }

    [MaxLength(128)]
    public string? TwoFactorSecret { get; set; }

    public bool TwoFactorEnabled { get; set; }
    public DateTime? TwoFactorConfirmedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant Tenant { get; set; } = null!;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = [];
    public ICollection<UserBackupCode> BackupCodes { get; set; } = [];

    [InverseProperty(nameof(Invitation.InvitedByUser))]
    public ICollection<Invitation> SentInvitations { get; set; } = [];

    public static User Create(
        Guid tenantId,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        UserRole role)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            IsActive = true,
            TwoFactorEnabled = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}
