using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_ReportType_ReportTypeName",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReports_ReportType_ReportTypeName",
                table: "MessageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReports_ReportType_ReportTypeName",
                table: "PostReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportType",
                table: "ReportType");

            migrationBuilder.RenameTable(
                name: "ReportType",
                newName: "ReportTypes");

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedAdminId",
                table: "PostReports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "PostReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PostReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedAdminId",
                table: "MessageReports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "MessageReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MessageReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedAdminId",
                table: "CommentReports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "CommentReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CommentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ReviewedAdminId",
                table: "PostReports",
                column: "ReviewedAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReports_ReviewedAdminId",
                table: "MessageReports",
                column: "ReviewedAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReviewedAdminId",
                table: "CommentReports",
                column: "ReviewedAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeName",
                table: "CommentReports",
                column: "ReportTypeName",
                principalTable: "ReportTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Users_ReviewedAdminId",
                table: "CommentReports",
                column: "ReviewedAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReports_ReportTypes_ReportTypeName",
                table: "MessageReports",
                column: "ReportTypeName",
                principalTable: "ReportTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReports_Users_ReviewedAdminId",
                table: "MessageReports",
                column: "ReviewedAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReports_ReportTypes_ReportTypeName",
                table: "PostReports",
                column: "ReportTypeName",
                principalTable: "ReportTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReports_Users_ReviewedAdminId",
                table: "PostReports",
                column: "ReviewedAdminId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeName",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Users_ReviewedAdminId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReports_ReportTypes_ReportTypeName",
                table: "MessageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReports_Users_ReviewedAdminId",
                table: "MessageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReports_ReportTypes_ReportTypeName",
                table: "PostReports");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReports_Users_ReviewedAdminId",
                table: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_PostReports_ReviewedAdminId",
                table: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_MessageReports_ReviewedAdminId",
                table: "MessageReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_ReviewedAdminId",
                table: "CommentReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes");

            migrationBuilder.DropColumn(
                name: "ReviewedAdminId",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "ReviewedAdminId",
                table: "MessageReports");

            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "MessageReports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MessageReports");

            migrationBuilder.DropColumn(
                name: "ReviewedAdminId",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CommentReports");

            migrationBuilder.RenameTable(
                name: "ReportTypes",
                newName: "ReportType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportType",
                table: "ReportType",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_ReportType_ReportTypeName",
                table: "CommentReports",
                column: "ReportTypeName",
                principalTable: "ReportType",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReports_ReportType_ReportTypeName",
                table: "MessageReports",
                column: "ReportTypeName",
                principalTable: "ReportType",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReports_ReportType_ReportTypeName",
                table: "PostReports",
                column: "ReportTypeName",
                principalTable: "ReportType",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
