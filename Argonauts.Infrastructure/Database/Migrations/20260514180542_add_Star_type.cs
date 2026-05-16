using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class add_Star_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Stars",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Stars");
        }
    }
}
