using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwestr.Modules.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitAnalyticsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "analytics");

            migrationBuilder.CreateTable(
                name: "analytics_insight_sets",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    provider = table.Column<int>(type: "integer", nullable: false),
                    trigger = table.Column<int>(type: "integer", nullable: false),
                    data_snapshot_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_analytics_insight_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "daily_engagement_snapshots",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    active_users = table.Column<int>(type: "integer", nullable: false),
                    steps_completed = table.Column<int>(type: "integer", nullable: false),
                    scenarios_started = table.Column<int>(type: "integer", nullable: false),
                    scenarios_completed = table.Column<int>(type: "integer", nullable: false),
                    badges_earned = table.Column<int>(type: "integer", nullable: false),
                    total_points_granted = table.Column<int>(type: "integer", nullable: false),
                    calculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_engagement_snapshots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scenario_assignment_facts",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scenario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    current_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scenario_assignment_facts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scenario_metadata_caches",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scenario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scenario_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scenario_metadata_caches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "step_completion_facts",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scenario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_type = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_spent = table.Column<TimeSpan>(type: "interval", nullable: false),
                    was_successful = table.Column<bool>(type: "boolean", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: true),
                    max_score = table.Column<int>(type: "integer", nullable: true),
                    attempt_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_step_completion_facts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "insights",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    metric_reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    insight_set_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_insights", x => x.id);
                    table.ForeignKey(
                        name: "fk_insights_analytics_insight_sets_insight_set_id",
                        column: x => x.insight_set_id,
                        principalSchema: "analytics",
                        principalTable: "analytics_insight_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recommendations",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    insight_set_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recommendations", x => x.id);
                    table.ForeignKey(
                        name: "fk_recommendations_analytics_insight_sets_insight_set_id",
                        column: x => x.insight_set_id,
                        principalSchema: "analytics",
                        principalTable: "analytics_insight_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stage_metadata_items",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stage_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    scenario_metadata_cache_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stage_metadata_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_stage_metadata_items_scenario_metadata_caches_scenario_meta",
                        column: x => x.scenario_metadata_cache_id,
                        principalSchema: "analytics",
                        principalTable: "scenario_metadata_caches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "step_metadata_items",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    stage_metadata_item_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_step_metadata_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_step_metadata_items_stage_metadata_items_stage_metadata_ite",
                        column: x => x.stage_metadata_item_id,
                        principalSchema: "analytics",
                        principalTable: "stage_metadata_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_analytics_insight_sets_tenant_id_data_snapshot_hash",
                schema: "analytics",
                table: "analytics_insight_sets",
                columns: new[] { "tenant_id", "data_snapshot_hash" });

            migrationBuilder.CreateIndex(
                name: "ix_analytics_insight_sets_tenant_id_generated_at",
                schema: "analytics",
                table: "analytics_insight_sets",
                columns: new[] { "tenant_id", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_analytics_insight_sets_tenant_id_trigger_generated_at",
                schema: "analytics",
                table: "analytics_insight_sets",
                columns: new[] { "tenant_id", "trigger", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_daily_engagement_snapshots_tenant_id_date",
                schema: "analytics",
                table: "daily_engagement_snapshots",
                columns: new[] { "tenant_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_insights_insight_set_id",
                schema: "analytics",
                table: "insights",
                column: "insight_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_recommendations_insight_set_id",
                schema: "analytics",
                table: "recommendations",
                column: "insight_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_scenario_assignment_facts_tenant_id_assigned_at",
                schema: "analytics",
                table: "scenario_assignment_facts",
                columns: new[] { "tenant_id", "assigned_at" });

            migrationBuilder.CreateIndex(
                name: "ix_scenario_assignment_facts_tenant_id_assignment_id",
                schema: "analytics",
                table: "scenario_assignment_facts",
                columns: new[] { "tenant_id", "assignment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_scenario_metadata_caches_tenant_id_scenario_id",
                schema: "analytics",
                table: "scenario_metadata_caches",
                columns: new[] { "tenant_id", "scenario_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stage_metadata_items_scenario_metadata_cache_id",
                schema: "analytics",
                table: "stage_metadata_items",
                column: "scenario_metadata_cache_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_completion_facts_tenant_id_completed_at",
                schema: "analytics",
                table: "step_completion_facts",
                columns: new[] { "tenant_id", "completed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_step_completion_facts_tenant_id_scenario_id_completed_at",
                schema: "analytics",
                table: "step_completion_facts",
                columns: new[] { "tenant_id", "scenario_id", "completed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_step_completion_facts_tenant_id_step_id",
                schema: "analytics",
                table: "step_completion_facts",
                columns: new[] { "tenant_id", "step_id" });

            migrationBuilder.CreateIndex(
                name: "ix_step_completion_facts_tenant_id_step_id_user_id_attempt_num",
                schema: "analytics",
                table: "step_completion_facts",
                columns: new[] { "tenant_id", "step_id", "user_id", "attempt_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_step_metadata_items_stage_metadata_item_id",
                schema: "analytics",
                table: "step_metadata_items",
                column: "stage_metadata_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_engagement_snapshots",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "insights",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "recommendations",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "scenario_assignment_facts",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "step_completion_facts",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "step_metadata_items",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "analytics_insight_sets",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "stage_metadata_items",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "scenario_metadata_caches",
                schema: "analytics");
        }
    }
}
