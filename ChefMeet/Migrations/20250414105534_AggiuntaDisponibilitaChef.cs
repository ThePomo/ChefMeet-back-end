using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefMeet.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntaDisponibilitaChef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisponibilitaChef",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChefId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OraInizio = table.Column<TimeSpan>(type: "time", nullable: false),
                    OraFine = table.Column<TimeSpan>(type: "time", nullable: false),
                    ÈDisponibile = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisponibilitaChef", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisponibilitaChef_Chefs_ChefId",
                        column: x => x.ChefId,
                        principalTable: "Chefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilitaChef_ChefId",
                table: "DisponibilitaChef",
                column: "ChefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisponibilitaChef");
        }
    }
}
