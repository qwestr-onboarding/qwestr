using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure;

public class AnalyticsDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("analytics");
    }
}