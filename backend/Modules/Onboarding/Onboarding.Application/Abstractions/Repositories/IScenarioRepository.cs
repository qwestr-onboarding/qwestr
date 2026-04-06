using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Modules.Onboarding.Application.Commands;

namespace Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;

public interface IScenarioRepository
{
    Task<Scenario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Scenario?> GetWithFullTreeAsync(Guid id, CancellationToken ct = default);
    Task<Scenario?> GetWithStagesAndStepsAsync(Guid id, CancellationToken ct = default);
    Task<List<Scenario>> GetByTenantAsync(Guid tenantId, int page, int pageSize, string? tagFilter, CancellationToken ct = default);
    Task<int> CountByTenantAsync(Guid tenantId, string? tagFilter, CancellationToken ct = default);

    Task<Stage?> GetStageByIdAsync(Guid stageId, CancellationToken ct = default);
    Task<Stage?> GetStageWithStepsAsync(Guid stageId, CancellationToken ct = default);
    Task<Step?> GetStepByIdAsync(Guid stepId, CancellationToken ct = default);
    Task<Step?> GetStepWithQuestionsAsync(Guid stepId, CancellationToken ct = default);
    Task<Question?> GetQuestionByIdAsync(Guid questionId, CancellationToken ct = default);
    Task<Question?> GetQuestionWithOptionsAsync(Guid questionId, CancellationToken ct = default);
    Task<QuestionOption?> GetOptionByIdAsync(Guid optionId, CancellationToken ct = default);

    Task<int> CountQuestionsInStepAsync(Guid stepId, CancellationToken ct = default);
    Task<int> GetMaxStageOrderAsync(Guid scenarioId, CancellationToken ct = default);
    Task<int> GetMaxStepOrderAsync(Guid stageId, CancellationToken ct = default);
    Task<int> GetMaxQuestionOrderAsync(Guid stepId, CancellationToken ct = default);
    Task<int> GetMaxOptionOrderAsync(Guid questionId, CancellationToken ct = default);

    Task<Stage?> GetAdjacentStageAsync(Guid scenarioId, int currentOrder, MoveDirection direction, CancellationToken ct = default);
    Task<Step?> GetAdjacentStepAsync(Guid stageId, int currentOrder, MoveDirection direction, CancellationToken ct = default);
    Task<Question?> GetAdjacentQuestionAsync(Guid stepId, int currentOrder, MoveDirection direction, CancellationToken ct = default);
    Task<QuestionOption?> GetAdjacentOptionAsync(Guid questionId, int currentOrder, MoveDirection direction, CancellationToken ct = default);

    Task<bool> HasActiveAssignmentsAsync(Guid scenarioId, CancellationToken ct = default);

    Task AddAsync(Scenario scenario, CancellationToken ct = default);
    Task AddStageAsync(Stage stage, CancellationToken ct = default);
    Task AddStepAsync(Step step, CancellationToken ct = default);
    Task AddQuestionAsync(Question question, CancellationToken ct = default);
    Task AddOptionAsync(QuestionOption option, CancellationToken ct = default);

    void Remove(Scenario scenario);
    void RemoveStage(Stage stage);
    void RemoveStep(Step step);
    void RemoveQuestion(Question question);
    void RemoveOption(QuestionOption option);

    Task SaveChangesAsync(CancellationToken ct = default);
}
