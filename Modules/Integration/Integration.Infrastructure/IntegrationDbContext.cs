using Microsoft.EntityFrameworkCore;

namespace Integration.Infrastructure;

public class IntegrationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");
    }
}