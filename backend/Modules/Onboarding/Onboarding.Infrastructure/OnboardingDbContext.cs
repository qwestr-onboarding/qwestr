using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Infrastructure;

namespace Qwestr.Modules.Onboarding.Infrastructure;

public class OnboardingDbContext : DbContext
{
    public OnboardingDbContext(DbContextOptions<OnboardingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Scenario> Scenarios => Set<Scenario>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<Step> Steps => Set<Step>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<ScenarioAssignment> ScenarioAssignments => Set<ScenarioAssignment>();
    public DbSet<StepProgress> StepProgresses => Set<StepProgress>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<QuestionAnswer> QuestionAnswers => Set<QuestionAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("onboarding");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ChangeTracker.ApplyAuditableEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.ApplyAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }
}
