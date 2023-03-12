using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    public partial class UpdateTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Linkedin",
                table: "Teams",
                newName: "Twitter");

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Teams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "Twitter",
                table: "Teams",
                newName: "Linkedin");
        }
    }
}
