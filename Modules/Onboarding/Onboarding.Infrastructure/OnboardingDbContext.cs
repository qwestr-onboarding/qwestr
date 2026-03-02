using Microsoft.EntityFrameworkCore;

namespace Onboarding.Infrastructure;

public class OnboardingDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("onboarding");
    }
}