using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefMeet.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSpecialitaFromChef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialità",
                table: "Chefs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specialità",
                table: "Chefs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
