using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefMeet.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntaRicettario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatoreId",
                table: "Creazioni",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Creazioni_CreatoreId",
                table: "Creazioni",
                column: "CreatoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Creazioni_AspNetUsers_CreatoreId",
                table: "Creazioni",
                column: "CreatoreId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Creazioni_AspNetUsers_CreatoreId",
                table: "Creazioni");

            migrationBuilder.DropIndex(
                name: "IX_Creazioni_CreatoreId",
                table: "Creazioni");

            migrationBuilder.DropColumn(
                name: "CreatoreId",
                table: "Creazioni");
        }
    }
}
