using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Kernel.Entities;

namespace Identity.Domain.Entities;

/// <summary>
/// Токен сброса пароля
/// </summary>
public sealed class PasswordResetToken : Entity<Guid>
{
    private PasswordResetToken() { }

    public Guid UserId { get; set; }

    [Required]
    [MaxLength(512)]
    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public static PasswordResetToken Create(Guid userId, string token, DateTime expiresAt)
    {
        return new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}
