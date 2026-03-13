using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Domain.Entities;

/// <summary>
/// JWT refresh-токен
/// </summary>
public sealed class RefreshToken : Entity<Guid>
{
    private RefreshToken() { }

    public Guid UserId { get; set; }

    [Required]
    [MaxLength(512)]
    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    [MaxLength(512)]
    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
