using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefMeet.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntaImmagineEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Immagine",
                table: "Eventi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Immagine",
                table: "Eventi");
        }
    }
}
