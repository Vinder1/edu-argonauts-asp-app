using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class Add_MaxDistance_and_Speed_to_SpaceshipCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Stars",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "-",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "MaxDistance",
                table: "SpaceshipConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Speed",
                table: "SpaceshipConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDistance",
                table: "SpaceshipConditions");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "SpaceshipConditions");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Stars",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "-");
        }
    }
}
