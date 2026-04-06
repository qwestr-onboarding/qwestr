using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Repositories;

public class ScenarioAssignmentRepository : IScenarioAssignmentRepository
{
    private readonly OnboardingDbContext _context;

    public ScenarioAssignmentRepository(OnboardingDbContext context)
    {
        _context = context;
    }

    public async Task<ScenarioAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.ScenarioAssignments.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<ScenarioAssignment?> GetWithProgressAsync(Guid id, CancellationToken ct = default)
        => await _context.ScenarioAssignments
            .Include(a => a.StepProgresses)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<ScenarioAssignment?> GetWithFullProgressAsync(Guid id, CancellationToken ct = default)
        => await _context.ScenarioAssignments
            .Include(a => a.StepProgresses)
                .ThenInclude(sp => sp.TestAttempts)
            .Include(a => a.StepProgresses)
                .ThenInclude(sp => sp.QuestionAnswers)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<List<ScenarioAssignment>> GetByUserAsync(Guid tenantId, Guid userId, CancellationToken ct = default)
        => await _context.ScenarioAssignments
            .Where(a => a.TenantId == tenantId && a.UserId == userId)
            .OrderByDescending(a => a.AssignedAt)
            .ToListAsync(ct);

    public async Task<List<ScenarioAssignment>> GetByTenantAsync(Guid tenantId, int page, int pageSize, Guid? scenarioId, Guid? userId, CancellationToken ct = default)
    {
        var query = _context.ScenarioAssignments.Where(a => a.TenantId == tenantId);

        if (scenarioId.HasValue)
        {
            query = query.Where(a => a.ScenarioId == scenarioId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        return await query
            .OrderByDescending(a => a.AssignedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountByTenantAsync(Guid tenantId, Guid? scenarioId, Guid? userId, CancellationToken ct = default)
    {
        var query = _context.ScenarioAssignments.Where(a => a.TenantId == tenantId);

        if (scenarioId.HasValue)
        {
            query = query.Where(a => a.ScenarioId == scenarioId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsActiveAsync(Guid tenantId, Guid scenarioId, Guid userId, CancellationToken ct = default)
        => await _context.ScenarioAssignments
            .AnyAsync(a => a.TenantId == tenantId && a.ScenarioId == scenarioId && a.UserId == userId
                           && a.Status != AssignmentStatus.Completed, ct);

    public async Task<StepProgress?> GetStepProgressAsync(Guid assignmentId, Guid stepId, CancellationToken ct = default)
        => await _context.StepProgresses
            .FirstOrDefaultAsync(sp => sp.AssignmentId == assignmentId && sp.StepId == stepId, ct);

    public async Task<StepProgress?> GetStepProgressByIdAsync(Guid stepProgressId, CancellationToken ct = default)
        => await _context.StepProgresses.FirstOrDefaultAsync(sp => sp.Id == stepProgressId, ct);

    public async Task<TestAttempt?> GetTestAttemptByIdAsync(Guid testAttemptId, CancellationToken ct = default)
        => await _context.TestAttempts.FirstOrDefaultAsync(t => t.Id == testAttemptId, ct);

    public async Task<TestAttempt?> GetTestAttemptWithAnswersAsync(Guid testAttemptId, CancellationToken ct = default)
        => await _context.TestAttempts
            .Include(t => t.QuestionAnswers)
            .FirstOrDefaultAsync(t => t.Id == testAttemptId, ct);

    public async Task<int> CountTestAttemptsAsync(Guid stepProgressId, CancellationToken ct = default)
        => await _context.TestAttempts.CountAsync(t => t.StepProgressId == stepProgressId, ct);

    public async Task<int> GetLastAttemptNumberAsync(Guid stepProgressId, CancellationToken ct = default)
        => await _context.TestAttempts
            .Where(t => t.StepProgressId == stepProgressId)
            .Select(t => (int?)t.AttemptNumber)
            .MaxAsync(ct) ?? 0;

    public async Task<Guid?> GetLatestIncompleteAttemptIdAsync(Guid stepProgressId, CancellationToken ct = default)
        => await _context.TestAttempts
            .Where(t => t.StepProgressId == stepProgressId && t.CompletedAt == null)
            .OrderByDescending(t => t.StartedAt)
            .Select(t => (Guid?)t.Id)
            .FirstOrDefaultAsync(ct);

    public async Task<string?> GetTestAttemptDraftAsync(Guid attemptId, CancellationToken ct = default)
        => await _context.TestAttempts
            .Where(t => t.Id == attemptId)
            .Select(t => t.DraftAnswersJson)
            .FirstOrDefaultAsync(ct);

    public async Task SaveTestAttemptDraftAsync(Guid attemptId, string? draftJson, CancellationToken ct = default)
    {
        var attempt = await _context.TestAttempts.FindAsync([attemptId], ct);
        if (attempt is null) return;
        attempt.SaveDraft(draftJson);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddAsync(ScenarioAssignment assignment, CancellationToken ct = default)
        => await _context.ScenarioAssignments.AddAsync(assignment, ct);

    public Task RemoveAsync(ScenarioAssignment assignment, CancellationToken ct = default)
    {
        _context.ScenarioAssignments.Remove(assignment);
        return Task.CompletedTask;
    }

    public async Task AddStepProgressAsync(StepProgress stepProgress, CancellationToken ct = default)
        => await _context.StepProgresses.AddAsync(stepProgress, ct);

    public async Task AddTestAttemptAsync(TestAttempt testAttempt, CancellationToken ct = default)
        => await _context.TestAttempts.AddAsync(testAttempt, ct);

    public async Task AddQuestionAnswerAsync(QuestionAnswer questionAnswer, CancellationToken ct = default)
        => await _context.QuestionAnswers.AddAsync(questionAnswer, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
