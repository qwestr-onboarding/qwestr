using Microsoft.EntityFrameworkCore;

namespace Gamification.Infrastructure;

public class GamificationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gamification");
    }
}