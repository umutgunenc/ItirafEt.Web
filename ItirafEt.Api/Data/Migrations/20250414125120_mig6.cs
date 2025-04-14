using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_BannedByUserId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "BannedByUserId",
                table: "Users",
                newName: "AdminastorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_BannedByUserId",
                table: "Users",
                newName: "IX_Users_AdminastorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_AdminastorUserId",
                table: "Users",
                column: "AdminastorUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_AdminastorUserId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AdminastorUserId",
                table: "Users",
                newName: "BannedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_AdminastorUserId",
                table: "Users",
                newName: "IX_Users_BannedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_BannedByUserId",
                table: "Users",
                column: "BannedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
