using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwestr.Modules.Onboarding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestAttemptDraftAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "draft_answers_json",
                schema: "onboarding",
                table: "test_attempts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "draft_answers_json",
                schema: "onboarding",
                table: "test_attempts");
        }
    }
}
