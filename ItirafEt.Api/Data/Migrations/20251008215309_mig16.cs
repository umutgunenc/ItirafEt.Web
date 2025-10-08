using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Comments_CommentId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeName",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Users_ReviewedAdminId",
                table: "CommentReports");

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "MessageReports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "CommentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CommentReports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReports_ConversationId",
                table: "MessageReports",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_PostId",
                table: "CommentReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_UserId",
                table: "CommentReports",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Comments_CommentId",
                table: "CommentReports",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Posts_PostId",
                table: "CommentReports",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeName",
                table: "CommentReports",
                column: "ReportTypeName",
                principalTable: "ReportTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Users_ReviewedAdminId",
                table: "CommentReports",
                column: "ReviewedAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Users_UserId",
                table: "CommentReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReports_Conversations_ConversationId",
                table: "MessageReports",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Comments_CommentId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Posts_PostId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeName",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Users_ReviewedAdminId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Users_UserId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReports_Conversations_ConversationId",
                table: "MessageReports");

            migrationBuilder.DropIndex(
                name: "IX_MessageReports_ConversationId",
                table: "MessageReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_PostId",
                table: "CommentReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_UserId",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "MessageReports");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CommentReports");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Comments_CommentId",
                table: "CommentReports",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
        }
    }
}
