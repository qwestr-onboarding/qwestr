using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;

public interface IScenarioAssignmentRepository
{
    Task<ScenarioAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ScenarioAssignment?> GetWithProgressAsync(Guid id, CancellationToken ct = default);
    Task<ScenarioAssignment?> GetWithFullProgressAsync(Guid id, CancellationToken ct = default);
    Task<List<ScenarioAssignment>> GetByUserAsync(Guid tenantId, Guid userId, CancellationToken ct = default);
    Task<List<ScenarioAssignment>> GetByTenantAsync(Guid tenantId, int page, int pageSize, Guid? scenarioId, Guid? userId, CancellationToken ct = default);
    Task<int> CountByTenantAsync(Guid tenantId, Guid? scenarioId, Guid? userId, CancellationToken ct = default);
    Task<bool> ExistsActiveAsync(Guid tenantId, Guid scenarioId, Guid userId, CancellationToken ct = default);

    Task<StepProgress?> GetStepProgressAsync(Guid assignmentId, Guid stepId, CancellationToken ct = default);
    Task<StepProgress?> GetStepProgressByIdAsync(Guid stepProgressId, CancellationToken ct = default);
    Task<TestAttempt?> GetTestAttemptByIdAsync(Guid testAttemptId, CancellationToken ct = default);
    Task<TestAttempt?> GetTestAttemptWithAnswersAsync(Guid testAttemptId, CancellationToken ct = default);
    Task<int> CountTestAttemptsAsync(Guid stepProgressId, CancellationToken ct = default);
    Task<int> GetLastAttemptNumberAsync(Guid stepProgressId, CancellationToken ct = default);
    Task<List<TestAttempt>> GetCompletedTestAttemptsAsync(Guid stepProgressId, CancellationToken ct = default);
    Task<Guid?> GetLatestIncompleteAttemptIdAsync(Guid stepProgressId, CancellationToken ct = default);
    Task<string?> GetTestAttemptDraftAsync(Guid attemptId, CancellationToken ct = default);
    Task SaveTestAttemptDraftAsync(Guid attemptId, string? draftJson, CancellationToken ct = default);

    Task AddAsync(ScenarioAssignment assignment, CancellationToken ct = default);
    Task RemoveAsync(ScenarioAssignment assignment, CancellationToken ct = default);
    Task AddStepProgressAsync(StepProgress stepProgress, CancellationToken ct = default);
    Task AddTestAttemptAsync(TestAttempt testAttempt, CancellationToken ct = default);
    Task AddQuestionAnswerAsync(QuestionAnswer questionAnswer, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
