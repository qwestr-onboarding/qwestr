using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Analytics;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Requests;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Client.Infrastructure.Api.Endpoints.Analytics;

public class AnalyticsApi(QwestrApiClient api)
{
    // ── Dashboard ──────────────────────────────────────────────────────────────

    public Task<ApiResult<DashboardResponse>> GetDashboardAsync(CancellationToken ct = default)
        => api.GetAsync<DashboardResponse>("/api/analytics/dashboard", ct);

    // ── Engagement ─────────────────────────────────────────────────────────────

    public Task<ApiResult<EngagementTrendResponse>> GetEngagementTrendAsync(
        int days = 30, CancellationToken ct = default)
        => api.GetAsync<EngagementTrendResponse>($"/api/analytics/engagement/trend?days={days}", ct);

    // ── Scenario analytics ─────────────────────────────────────────────────────

    public Task<ApiResult<ScenarioFunnelResponse>> GetScenarioFunnelAsync(
        Guid scenarioId, CancellationToken ct = default)
        => api.GetAsync<ScenarioFunnelResponse>($"/api/analytics/scenarios/{scenarioId}/funnel", ct);

    public Task<ApiResult<ContentQualityReportResponse>> GetContentQualityAsync(
        Guid scenarioId, CancellationToken ct = default)
        => api.GetAsync<ContentQualityReportResponse>(
            $"/api/analytics/scenarios/{scenarioId}/content-quality", ct);

    // ── AI Insights ────────────────────────────────────────────────────────────

    public Task<ApiResult<AnalyticsInsightSetResponse>> GetLatestInsightsAsync(
        CancellationToken ct = default)
        => api.GetAsync<AnalyticsInsightSetResponse>("/api/analytics/insights/latest", ct);

    public Task<ApiResult<AnalyticsInsightSetResponse>> GenerateInsightsAsync(
        LlmProvider provider, CancellationToken ct = default)
        => api.PostAsync<AnalyticsInsightSetResponse>(
            "/api/analytics/insights/generate",
            new GenerateInsightsRequest(provider),
            ct);

    public Task<ApiResult<CooldownStatusResponse>> GetCooldownStatusAsync(
        CancellationToken ct = default)
        => api.GetAsync<CooldownStatusResponse>("/api/analytics/insights/cooldown", ct);

    // ── Cohort retention ───────────────────────────────────────────────────────

    public Task<ApiResult<CohortRetentionResponse>> GetCohortRetentionAsync(
        int weeks = 8, CancellationToken ct = default)
        => api.GetAsync<CohortRetentionResponse>(
            $"/api/analytics/cohorts/retention?weeks={weeks}", ct);

    // ── Export ─────────────────────────────────────────────────────────────────

    public Task<ApiResult<byte[]>> ExportCsvAsync(
        DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var url = "/api/analytics/export?format=csv";
        if (from.HasValue) url += $"&from={from.Value:o}";
        if (to.HasValue)   url += $"&to={to.Value:o}";
        return api.GetBytesAsync(url, ct);
    }
}
