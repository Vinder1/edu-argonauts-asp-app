using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class add_Stat_values_in_SpaceshipCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAntimatter",
                table: "SpaceshipConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxDurability",
                table: "SpaceshipConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxEnergy",
                table: "SpaceshipConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Power",
                table: "SpaceshipConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAntimatter",
                table: "SpaceshipConditions");

            migrationBuilder.DropColumn(
                name: "MaxDurability",
                table: "SpaceshipConditions");

            migrationBuilder.DropColumn(
                name: "MaxEnergy",
                table: "SpaceshipConditions");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "SpaceshipConditions");
        }
    }
}
