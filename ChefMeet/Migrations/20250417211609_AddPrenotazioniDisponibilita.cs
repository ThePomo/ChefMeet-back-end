using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefMeet.Migrations
{
    /// <inheritdoc />
    public partial class AddPrenotazioniDisponibilita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrenotazioniDisponibilita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisponibilitaChefId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtenteId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DataPrenotazione = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrenotazioniDisponibilita", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrenotazioniDisponibilita_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrenotazioniDisponibilita_DisponibilitaChef_DisponibilitaChefId",
                        column: x => x.DisponibilitaChefId,
                        principalTable: "DisponibilitaChef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrenotazioniDisponibilita_DisponibilitaChefId",
                table: "PrenotazioniDisponibilita",
                column: "DisponibilitaChefId");

            migrationBuilder.CreateIndex(
                name: "IX_PrenotazioniDisponibilita_UtenteId",
                table: "PrenotazioniDisponibilita",
                column: "UtenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrenotazioniDisponibilita");
        }
    }
}
