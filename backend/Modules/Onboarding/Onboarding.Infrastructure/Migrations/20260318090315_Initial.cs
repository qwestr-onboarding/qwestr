using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwestr.Modules.Onboarding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onboarding");

            migrationBuilder.CreateTable(
                name: "scenario_assignments",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    scenario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_by = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scenario_assignments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scenarios",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scenarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "step_progresses",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    checked_items = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_step_progresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_step_progresses_scenario_assignments_assignment_id",
                        column: x => x.assignment_id,
                        principalSchema: "onboarding",
                        principalTable: "scenario_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stages",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    scenario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stages", x => x.id);
                    table.ForeignKey(
                        name: "fk_stages_scenarios_scenario_id",
                        column: x => x.scenario_id,
                        principalSchema: "onboarding",
                        principalTable: "scenarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_attempts",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_progress_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attempt_number = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: true),
                    is_passed = table.Column<bool>(type: "boolean", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_attempts_step_progresses_step_progress_id",
                        column: x => x.step_progress_id,
                        principalSchema: "onboarding",
                        principalTable: "step_progresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "steps",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "jsonb", nullable: true),
                    settings = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_steps_stages_stage_id",
                        column: x => x.stage_id,
                        principalSchema: "onboarding",
                        principalTable: "stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_answers",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_progress_id = table.Column<Guid>(type: "uuid", nullable: true),
                    test_attempt_id = table.Column<Guid>(type: "uuid", nullable: true),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    selected_option_ids = table.Column<List<Guid>>(type: "uuid[]", nullable: true),
                    text_answer = table.Column<string>(type: "text", nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    answered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_answers_step_progresses_step_progress_id",
                        column: x => x.step_progress_id,
                        principalSchema: "onboarding",
                        principalTable: "step_progresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_answers_test_attempts_test_attempt_id",
                        column: x => x.test_attempt_id,
                        principalSchema: "onboarding",
                        principalTable: "test_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    question_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_steps_step_id",
                        column: x => x.step_id,
                        principalSchema: "onboarding",
                        principalTable: "steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_options",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_options_questions_question_id",
                        column: x => x.question_id,
                        principalSchema: "onboarding",
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_question_answers_question_id",
                schema: "onboarding",
                table: "question_answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_answers_step_progress_id",
                schema: "onboarding",
                table: "question_answers",
                column: "step_progress_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_answers_test_attempt_id",
                schema: "onboarding",
                table: "question_answers",
                column: "test_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_options_question_id",
                schema: "onboarding",
                table: "question_options",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_step_id",
                schema: "onboarding",
                table: "questions",
                column: "step_id");

            migrationBuilder.CreateIndex(
                name: "ix_scenario_assignments_scenario_id",
                schema: "onboarding",
                table: "scenario_assignments",
                column: "scenario_id");

            migrationBuilder.CreateIndex(
                name: "ix_scenario_assignments_tenant_id_user_id",
                schema: "onboarding",
                table: "scenario_assignments",
                columns: new[] { "tenant_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_scenarios_tenant_id",
                schema: "onboarding",
                table: "scenarios",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_scenarios_tenant_id_is_published",
                schema: "onboarding",
                table: "scenarios",
                columns: new[] { "tenant_id", "is_published" });

            migrationBuilder.CreateIndex(
                name: "ix_stages_scenario_id",
                schema: "onboarding",
                table: "stages",
                column: "scenario_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_progresses_assignment_id_step_id",
                schema: "onboarding",
                table: "step_progresses",
                columns: new[] { "assignment_id", "step_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_steps_stage_id",
                schema: "onboarding",
                table: "steps",
                column: "stage_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_attempts_step_progress_id",
                schema: "onboarding",
                table: "test_attempts",
                column: "step_progress_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "question_answers",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "question_options",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "test_attempts",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "step_progresses",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "steps",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "scenario_assignments",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "stages",
                schema: "onboarding");

            migrationBuilder.DropTable(
                name: "scenarios",
                schema: "onboarding");
        }
    }
}
