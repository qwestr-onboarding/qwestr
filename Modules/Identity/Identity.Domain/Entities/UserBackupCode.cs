using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Kernel.Entities;

namespace Identity.Domain.Entities;

/// <summary>
/// Резервный код 2FA
/// </summary>
public sealed class UserBackupCode : Entity<Guid>
{
    private UserBackupCode() { }

    public Guid UserId { get; set; }

    [Required]
    [MaxLength(256)]
    public string CodeHash { get; set; } = null!;

    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public static UserBackupCode Create(Guid userId, string codeHash)
    {
        return new UserBackupCode
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CodeHash = codeHash,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}
