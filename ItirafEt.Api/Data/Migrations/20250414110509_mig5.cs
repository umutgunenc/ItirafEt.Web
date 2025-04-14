using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_BannedOrDeletedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedOrDeletedDate",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "BannedOrDeletedByUserId",
                table: "Users",
                newName: "BannedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_BannedOrDeletedByUserId",
                table: "Users",
                newName: "IX_Users_BannedByUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedDateUntil",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                columns: new[] { "BannedDate", "BannedDateUntil", "IsBanned", "IsDeleted" },
                values: new object[] { null, null, false, false });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_BannedByUserId",
                table: "Users",
                column: "BannedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_BannedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedDateUntil",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "BannedByUserId",
                table: "Users",
                newName: "BannedOrDeletedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_BannedByUserId",
                table: "Users",
                newName: "IX_Users_BannedOrDeletedByUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedOrDeletedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                columns: new[] { "BannedOrDeletedDate", "IsActive" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_BannedOrDeletedByUserId",
                table: "Users",
                column: "BannedOrDeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
