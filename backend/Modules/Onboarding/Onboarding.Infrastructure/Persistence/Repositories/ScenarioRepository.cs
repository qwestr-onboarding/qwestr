using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Application.Commands;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Repositories;

public class ScenarioRepository : IScenarioRepository
{
    private readonly OnboardingDbContext _context;

    public ScenarioRepository(OnboardingDbContext context)
    {
        _context = context;
    }

    public async Task<Scenario?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Scenarios.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Scenario?> GetWithFullTreeAsync(Guid id, CancellationToken ct = default)
        => await _context.Scenarios
            .Include(s => s.Stages.OrderBy(st => st.Order))
                .ThenInclude(st => st.Steps.OrderBy(sp => sp.Order))
                    .ThenInclude(sp => sp.Questions.OrderBy(q => q.Order))
                        .ThenInclude(q => q.Options.OrderBy(o => o.Order))
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Scenario?> GetWithStagesAndStepsAsync(Guid id, CancellationToken ct = default)
        => await _context.Scenarios
            .Include(s => s.Stages.OrderBy(st => st.Order))
                .ThenInclude(st => st.Steps.OrderBy(sp => sp.Order))
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<List<Scenario>> GetByTenantAsync(Guid tenantId, int page, int pageSize, string? tagFilter, CancellationToken ct = default)
    {
        var query = _context.Scenarios
            .Include(s => s.Stages)
                .ThenInclude(st => st.Steps)
            .Where(s => s.TenantId == tenantId);

        if (!string.IsNullOrEmpty(tagFilter))
        {
            query = query.Where(s => s.Tags.Contains(tagFilter));
        }

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountByTenantAsync(Guid tenantId, string? tagFilter, CancellationToken ct = default)
    {
        var query = _context.Scenarios.Where(s => s.TenantId == tenantId);
        if (!string.IsNullOrEmpty(tagFilter))
        {
            query = query.Where(s => s.Tags.Contains(tagFilter));
        }
        return await query.CountAsync(ct);
    }

    public async Task<Stage?> GetStageByIdAsync(Guid stageId, CancellationToken ct = default)
        => await _context.Stages.FirstOrDefaultAsync(s => s.Id == stageId, ct);

    public async Task<Stage?> GetStageWithStepsAsync(Guid stageId, CancellationToken ct = default)
        => await _context.Stages
            .Include(s => s.Steps)
                .ThenInclude(st => st.Questions)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(s => s.Id == stageId, ct);

    public async Task<Step?> GetStepByIdAsync(Guid stepId, CancellationToken ct = default)
        => await _context.Steps.FirstOrDefaultAsync(s => s.Id == stepId, ct);

    public async Task<Step?> GetStepWithQuestionsAsync(Guid stepId, CancellationToken ct = default)
        => await _context.Steps
            .Include(s => s.Questions.OrderBy(q => q.Order))
                .ThenInclude(q => q.Options.OrderBy(o => o.Order))
            .FirstOrDefaultAsync(s => s.Id == stepId, ct);

    public async Task<Question?> GetQuestionByIdAsync(Guid questionId, CancellationToken ct = default)
        => await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionId, ct);

    public async Task<Question?> GetQuestionWithOptionsAsync(Guid questionId, CancellationToken ct = default)
        => await _context.Questions
            .Include(q => q.Options.OrderBy(o => o.Order))
            .FirstOrDefaultAsync(q => q.Id == questionId, ct);

    public async Task<QuestionOption?> GetOptionByIdAsync(Guid optionId, CancellationToken ct = default)
        => await _context.QuestionOptions.FirstOrDefaultAsync(o => o.Id == optionId, ct);

    public async Task<int> CountQuestionsInStepAsync(Guid stepId, CancellationToken ct = default)
        => await _context.Questions.CountAsync(q => q.StepId == stepId, ct);

    public async Task<int> GetMaxStageOrderAsync(Guid scenarioId, CancellationToken ct = default)
        => await _context.Stages
            .Where(s => s.ScenarioId == scenarioId)
            .Select(s => (int?)s.Order)
            .MaxAsync(ct) ?? 0;

    public async Task<int> GetMaxStepOrderAsync(Guid stageId, CancellationToken ct = default)
        => await _context.Steps
            .Where(s => s.StageId == stageId)
            .Select(s => (int?)s.Order)
            .MaxAsync(ct) ?? 0;

    public async Task<int> GetMaxQuestionOrderAsync(Guid stepId, CancellationToken ct = default)
        => await _context.Questions
            .Where(q => q.StepId == stepId)
            .Select(q => (int?)q.Order)
            .MaxAsync(ct) ?? 0;

    public async Task<int> GetMaxOptionOrderAsync(Guid questionId, CancellationToken ct = default)
        => await _context.QuestionOptions
            .Where(o => o.QuestionId == questionId)
            .Select(o => (int?)o.Order)
            .MaxAsync(ct) ?? 0;

    public async Task<Stage?> GetAdjacentStageAsync(Guid scenarioId, int currentOrder, MoveDirection direction, CancellationToken ct = default)
    {
        if (direction == MoveDirection.Up)
        {
            return await _context.Stages
                .Where(s => s.ScenarioId == scenarioId && s.Order < currentOrder)
                .OrderByDescending(s => s.Order)
                .FirstOrDefaultAsync(ct);
        }
        else
        {
            return await _context.Stages
                .Where(s => s.ScenarioId == scenarioId && s.Order > currentOrder)
                .OrderBy(s => s.Order)
                .FirstOrDefaultAsync(ct);
        }
    }

    public async Task<Step?> GetAdjacentStepAsync(Guid stageId, int currentOrder, MoveDirection direction, CancellationToken ct = default)
    {
        if (direction == MoveDirection.Up)
        {
            return await _context.Steps
                .Where(s => s.StageId == stageId && s.Order < currentOrder)
                .OrderByDescending(s => s.Order)
                .FirstOrDefaultAsync(ct);
        }
        else
        {
            return await _context.Steps
                .Where(s => s.StageId == stageId && s.Order > currentOrder)
                .OrderBy(s => s.Order)
                .FirstOrDefaultAsync(ct);
        }
    }

    public async Task<Question?> GetAdjacentQuestionAsync(Guid stepId, int currentOrder, MoveDirection direction, CancellationToken ct = default)
    {
        if (direction == MoveDirection.Up)
        {
            return await _context.Questions
                .Where(q => q.StepId == stepId && q.Order < currentOrder)
                .OrderByDescending(q => q.Order)
                .FirstOrDefaultAsync(ct);
        }
        else
        {
            return await _context.Questions
                .Where(q => q.StepId == stepId && q.Order > currentOrder)
                .OrderBy(q => q.Order)
                .FirstOrDefaultAsync(ct);
        }
    }

    public async Task<QuestionOption?> GetAdjacentOptionAsync(Guid questionId, int currentOrder, MoveDirection direction, CancellationToken ct = default)
    {
        if (direction == MoveDirection.Up)
        {
            return await _context.QuestionOptions
                .Where(o => o.QuestionId == questionId && o.Order < currentOrder)
                .OrderByDescending(o => o.Order)
                .FirstOrDefaultAsync(ct);
        }
        else
        {
            return await _context.QuestionOptions
                .Where(o => o.QuestionId == questionId && o.Order > currentOrder)
                .OrderBy(o => o.Order)
                .FirstOrDefaultAsync(ct);
        }
    }

    public async Task<bool> HasActiveAssignmentsAsync(Guid scenarioId, CancellationToken ct = default)
        => await _context.ScenarioAssignments
            .AnyAsync(a => a.ScenarioId == scenarioId && a.Status != AssignmentStatus.Completed, ct);

    public async Task AddAsync(Scenario scenario, CancellationToken ct = default)
        => await _context.Scenarios.AddAsync(scenario, ct);

    public async Task AddStageAsync(Stage stage, CancellationToken ct = default)
        => await _context.Stages.AddAsync(stage, ct);

    public async Task AddStepAsync(Step step, CancellationToken ct = default)
        => await _context.Steps.AddAsync(step, ct);

    public async Task AddQuestionAsync(Question question, CancellationToken ct = default)
        => await _context.Questions.AddAsync(question, ct);

    public async Task AddOptionAsync(QuestionOption option, CancellationToken ct = default)
        => await _context.QuestionOptions.AddAsync(option, ct);

    public void RemoveStage(Stage stage)
        => _context.Stages.Remove(stage);

    public void RemoveStep(Step step)
        => _context.Steps.Remove(step);

    public void RemoveQuestion(Question question)
        => _context.Questions.Remove(question);

    public void RemoveOption(QuestionOption option)
        => _context.QuestionOptions.Remove(option);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
