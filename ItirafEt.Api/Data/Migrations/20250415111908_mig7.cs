using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_ReportType_ReportTypeId",
                table: "MessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReaction_ReactionType_ReactionTypeId",
                table: "PostReaction");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReaction_ReportType_ReportTypeId",
                table: "PostReaction");

            migrationBuilder.DropIndex(
                name: "IX_PostReaction_ReactingUserId",
                table: "PostReaction");

            migrationBuilder.DropIndex(
                name: "IX_PostReaction_ReportTypeId",
                table: "PostReaction");

            migrationBuilder.DropIndex(
                name: "IX_MessageReactions_ReactingUserId",
                table: "MessageReactions");

            migrationBuilder.DropIndex(
                name: "IX_MessageReactions_ReportTypeId",
                table: "MessageReactions");

            migrationBuilder.DropIndex(
                name: "IX_CommentReactions_ReactingUserId",
                table: "CommentReactions");

            migrationBuilder.DropColumn(
                name: "ReportExplanation",
                table: "PostReaction");

            migrationBuilder.DropColumn(
                name: "ReportTypeId",
                table: "PostReaction");

            migrationBuilder.DropColumn(
                name: "ReportTypeId",
                table: "MessageReactions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ReactionType",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DislikeCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ReactionTypeId",
                table: "PostReaction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DislikeCount",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CommentReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportingUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: false),
                    ReportExplanation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentReports_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReports_ReportType_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReports_Users_ReportingUserId",
                        column: x => x.ReportingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportingUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: false),
                    ReportExplanation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReports_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReports_ReportType_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReports_Users_ReportingUserId",
                        column: x => x.ReportingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportingUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: false),
                    ReportExplanation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReports_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReports_ReportType_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReports_Users_ReportingUserId",
                        column: x => x.ReportingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_ReactingUserId_PostId",
                table: "PostReaction",
                columns: new[] { "ReactingUserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_ReactingUserId_MessageId",
                table: "MessageReactions",
                columns: new[] { "ReactingUserId", "MessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReactingUserId_CommentId",
                table: "CommentReactions",
                columns: new[] { "ReactingUserId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_CommentId",
                table: "CommentReports",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReportingUserId",
                table: "CommentReports",
                column: "ReportingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReportTypeId",
                table: "CommentReports",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReports_MessageId",
                table: "MessageReports",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReports_ReportingUserId",
                table: "MessageReports",
                column: "ReportingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReports_ReportTypeId",
                table: "MessageReports",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_PostId",
                table: "PostReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ReportingUserId",
                table: "PostReports",
                column: "ReportingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ReportTypeId",
                table: "PostReports",
                column: "ReportTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReaction_ReactionType_ReactionTypeId",
                table: "PostReaction",
                column: "ReactionTypeId",
                principalTable: "ReactionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostReaction_ReactionType_ReactionTypeId",
                table: "PostReaction");

            migrationBuilder.DropTable(
                name: "CommentReports");

            migrationBuilder.DropTable(
                name: "MessageReports");

            migrationBuilder.DropTable(
                name: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_PostReaction_ReactingUserId_PostId",
                table: "PostReaction");

            migrationBuilder.DropIndex(
                name: "IX_MessageReactions_ReactingUserId_MessageId",
                table: "MessageReactions");

            migrationBuilder.DropIndex(
                name: "IX_CommentReactions_ReactingUserId_CommentId",
                table: "CommentReactions");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "DislikeCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DislikeCount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ReactionType",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<int>(
                name: "ReactionTypeId",
                table: "PostReaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ReportExplanation",
                table: "PostReaction",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportTypeId",
                table: "PostReaction",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportTypeId",
                table: "MessageReactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_ReactingUserId",
                table: "PostReaction",
                column: "ReactingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_ReportTypeId",
                table: "PostReaction",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_ReactingUserId",
                table: "MessageReactions",
                column: "ReactingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_ReportTypeId",
                table: "MessageReactions",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReactingUserId",
                table: "CommentReactions",
                column: "ReactingUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_ReportType_ReportTypeId",
                table: "MessageReactions",
                column: "ReportTypeId",
                principalTable: "ReportType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReaction_ReactionType_ReactionTypeId",
                table: "PostReaction",
                column: "ReactionTypeId",
                principalTable: "ReactionType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReaction_ReportType_ReportTypeId",
                table: "PostReaction",
                column: "ReportTypeId",
                principalTable: "ReportType",
                principalColumn: "Id");
        }
    }
}
