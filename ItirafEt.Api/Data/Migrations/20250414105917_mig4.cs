using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BannedOrDeletedByUserId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                column: "BannedOrDeletedByUserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BannedOrDeletedByUserId",
                table: "Users",
                column: "BannedOrDeletedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_BannedOrDeletedByUserId",
                table: "Users",
                column: "BannedOrDeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_BannedOrDeletedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BannedOrDeletedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedOrDeletedByUserId",
                table: "Users");
        }
    }
}
