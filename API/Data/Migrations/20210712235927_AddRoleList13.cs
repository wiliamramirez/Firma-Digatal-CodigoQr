using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddRoleList13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_AppUserId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_DocumentDetails_DocumentId",
                table: "DocumentDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppUserId",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetails_DocumentId",
                table: "DocumentDetails",
                column: "DocumentId",
                unique: true);

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
                name: "IX_DocumentDetails_DocumentId",
                table: "DocumentDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppUserId",
                table: "Documents",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetails_DocumentId",
                table: "DocumentDetails",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_AppUserId",
                table: "Documents",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
