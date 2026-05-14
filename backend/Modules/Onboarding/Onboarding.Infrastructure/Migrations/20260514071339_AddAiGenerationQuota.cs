using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwestr.Modules.Onboarding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiGenerationQuota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_generation_quotas",
                schema: "onboarding",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    generation_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_generation_quotas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ai_generation_quotas_tenant_id_date",
                schema: "onboarding",
                table: "ai_generation_quotas",
                columns: new[] { "tenant_id", "date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_generation_quotas",
                schema: "onboarding");
        }
    }
}
