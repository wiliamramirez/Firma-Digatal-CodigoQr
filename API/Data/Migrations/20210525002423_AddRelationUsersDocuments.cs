using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddRelationUsersDocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AppUserId",
                table: "Documents",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_AppUserId",
                table: "Documents",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_AppUserId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_AppUserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Documents");
        }
    }
}
