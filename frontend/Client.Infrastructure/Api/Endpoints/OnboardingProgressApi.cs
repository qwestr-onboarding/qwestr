using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

namespace Client.Infrastructure.Api.Endpoints;

public class OnboardingProgressApi(QwestrApiClient api)
{
    public Task<ApiResult<List<MyAssignmentSummaryResponse>>> GetMyAssignmentsAsync(
        CancellationToken ct = default)
        => api.GetAsync<List<MyAssignmentSummaryResponse>>("/api/onboarding/my/assignments", ct);

    public Task<ApiResult<MyAssignmentProgressResponse>> GetMyAssignmentProgressAsync(
        Guid assignmentId, CancellationToken ct = default)
        => api.GetAsync<MyAssignmentProgressResponse>(
            $"/api/onboarding/my/assignments/{assignmentId}/progress", ct);

    public Task<ApiResult<StepContentResponse>> GetStepAsync(
        Guid assignmentId, Guid stepId, CancellationToken ct = default)
        => api.GetAsync<StepContentResponse>(
            $"/api/onboarding/my/assignments/{assignmentId}/steps/{stepId}", ct);

    public Task<ApiResult> StartStepAsync(
        Guid assignmentId, Guid stepId, CancellationToken ct = default)
        => api.PostAsync(
            $"/api/onboarding/my/assignments/{assignmentId}/steps/{stepId}/start", new { }, ct);

    public Task<ApiResult> CompleteInformationStepAsync(
        Guid progressId, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/my/progress/{progressId}/complete-information", new { }, ct);

    public Task<ApiResult> CompleteTaskStepAsync(
        Guid progressId, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/my/progress/{progressId}/complete-task", new { }, ct);

    public Task<ApiResult> UpdateChecklistProgressAsync(
        Guid progressId, UpdateChecklistProgressRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/onboarding/my/progress/{progressId}/checklist", request, ct);

    public Task<ApiResult> CompleteChecklistStepAsync(
        Guid progressId, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/my/progress/{progressId}/complete-checklist", new { }, ct);

    public Task<ApiResult<AnswerResultResponse>> AnswerQuestionAsync(
        Guid progressId, AnswerQuestionStepRequest request, CancellationToken ct = default)
        => api.PostAsync<AnswerResultResponse>(
            $"/api/onboarding/my/progress/{progressId}/answer", request, ct);

    public Task<ApiResult<IdResponse>> StartTestAttemptAsync(
        Guid progressId, CancellationToken ct = default)
        => api.PostAsync<IdResponse>(
            $"/api/onboarding/my/progress/{progressId}/test/start", new { }, ct);

    public Task<ApiResult<TestResultResponse>> SubmitTestAnswersAsync(
        Guid attemptId, SubmitTestAnswersRequest request, CancellationToken ct = default)
        => api.PostAsync<TestResultResponse>(
            $"/api/onboarding/my/test/{attemptId}/submit", request, ct);

    public Task<ApiResult> SaveTestDraftAnswersAsync(
        Guid attemptId, SaveTestDraftAnswersRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/onboarding/my/test/{attemptId}/draft", request, ct);

    public Task<ApiResult<TestResultResponse>> GetTestResultAsync(
        Guid attemptId, CancellationToken ct = default)
        => api.GetAsync<TestResultResponse>($"/api/onboarding/my/test/{attemptId}/result", ct);
}