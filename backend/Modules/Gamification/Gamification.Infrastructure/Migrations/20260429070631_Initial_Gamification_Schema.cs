using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwestr.Modules.Gamification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Gamification_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gamification");

            migrationBuilder.CreateTable(
                name: "achievement_progresses",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    award_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_value = table.Column<int>(type: "integer", nullable: false),
                    target_value = table.Column<int>(type: "integer", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_achievement_progresses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "awards",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    issuance_type = table.Column<int>(type: "integer", nullable: false),
                    rule = table.Column<string>(type: "jsonb", nullable: true),
                    xp_bonus = table.Column<int>(type: "integer", nullable: false),
                    coins_bonus = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_awards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coin_transactions",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false),
                    source_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coin_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "earned_awards",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    award_id = table.Column<Guid>(type: "uuid", nullable: false),
                    earned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    issued_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_earned_awards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_items",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shop_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false),
                    acquired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "level_tiers",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    from_level = table.Column<int>(type: "integer", nullable: false),
                    to_level = table.Column<int>(type: "integer", nullable: false),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    frame_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_level_tiers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_profiles",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    total_xp = table.Column<int>(type: "integer", nullable: false),
                    available_coins = table.Column<int>(type: "integer", nullable: false),
                    current_level = table.Column<int>(type: "integer", nullable: false),
                    equipment_profile_color_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    equipment_avatar_frame_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    equipment_background_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    equipment_pet_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    equipment_class_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    equipment_weapon_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_activity_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "points_transactions",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    xp_amount = table.Column<int>(type: "integer", nullable: false),
                    coins_amount = table.Column<int>(type: "integer", nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_points_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shop_items",
                schema: "gamification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slot = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    asset_data = table.Column<string>(type: "jsonb", nullable: false),
                    price = table.Column<int>(type: "integer", nullable: false),
                    required_level = table.Column<int>(type: "integer", nullable: true),
                    stacking_policy = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shop_items", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_achievement_progresses_profile_id",
                schema: "gamification",
                table: "achievement_progresses",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_achievement_progresses_profile_id_award_id",
                schema: "gamification",
                table: "achievement_progresses",
                columns: new[] { "profile_id", "award_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_awards_tenant_id",
                schema: "gamification",
                table: "awards",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_awards_tenant_id_is_active",
                schema: "gamification",
                table: "awards",
                columns: new[] { "tenant_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_coin_transactions_tenant_id_profile_id_occurred_at",
                schema: "gamification",
                table: "coin_transactions",
                columns: new[] { "tenant_id", "profile_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_earned_awards_profile_id",
                schema: "gamification",
                table: "earned_awards",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_earned_awards_profile_id_award_id",
                schema: "gamification",
                table: "earned_awards",
                columns: new[] { "profile_id", "award_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_profile_id",
                schema: "gamification",
                table: "inventory_items",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_profile_id_shop_item_id",
                schema: "gamification",
                table: "inventory_items",
                columns: new[] { "profile_id", "shop_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_level_tiers_tenant_id_order",
                schema: "gamification",
                table: "level_tiers",
                columns: new[] { "tenant_id", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_player_profiles_tenant_id_total_xp",
                schema: "gamification",
                table: "player_profiles",
                columns: new[] { "tenant_id", "total_xp" });

            migrationBuilder.CreateIndex(
                name: "ix_player_profiles_tenant_id_user_id",
                schema: "gamification",
                table: "player_profiles",
                columns: new[] { "tenant_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_points_transactions_idempotency",
                schema: "gamification",
                table: "points_transactions",
                columns: new[] { "profile_id", "source_type", "source_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_points_transactions_tenant_id_profile_id_occurred_at",
                schema: "gamification",
                table: "points_transactions",
                columns: new[] { "tenant_id", "profile_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_shop_items_tenant_id_is_active",
                schema: "gamification",
                table: "shop_items",
                columns: new[] { "tenant_id", "is_active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "achievement_progresses",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "awards",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "coin_transactions",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "earned_awards",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "inventory_items",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "level_tiers",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "player_profiles",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "points_transactions",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "shop_items",
                schema: "gamification");
        }
    }
}
