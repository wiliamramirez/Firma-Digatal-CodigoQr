using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddRoleList14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Affair",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "HashSecret",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Documents");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "DocumentDetails",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "DocumentDetails");

            migrationBuilder.AddColumn<string>(
                name: "Affair",
                table: "Documents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashSecret",
                table: "Documents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Documents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Documents",
                type: "TEXT",
                nullable: true);
        }
    }
}
