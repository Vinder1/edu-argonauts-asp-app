using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Argonauts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_quests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Killed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.OwnerId);
                    table.ForeignKey(
                        name: "FK_Quests_Spaceships_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Spaceships",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quests");
        }
    }
}
