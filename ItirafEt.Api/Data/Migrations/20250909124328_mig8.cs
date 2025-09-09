using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentHistories_Comments_ParentCommentId",
                table: "CommentHistories");

            migrationBuilder.DropIndex(
                name: "IX_CommentHistories_ParentCommentId",
                table: "CommentHistories");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PostHistories");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CommentHistories");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "CommentHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PostHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CommentHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "CommentHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistories_ParentCommentId",
                table: "CommentHistories",
                column: "ParentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentHistories_Comments_ParentCommentId",
                table: "CommentHistories",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
