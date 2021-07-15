using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddHashSha256 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HashSecret",
                table: "DocumentDetails",
                newName: "HashSecretSha256");

            migrationBuilder.AddColumn<string>(
                name: "HashSecretMD5",
                table: "DocumentDetails",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashSecretMD5",
                table: "DocumentDetails");

            migrationBuilder.RenameColumn(
                name: "HashSecretSha256",
                table: "DocumentDetails",
                newName: "HashSecret");
        }
    }
}
