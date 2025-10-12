using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeName",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReports_ReportTypes_ReportTypeName",
                table: "MessageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReports_ReportTypes_ReportTypeName",
                table: "PostReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes");

            migrationBuilder.DropIndex(
                name: "IX_PostReports_ReportTypeName",
                table: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_MessageReports_ReportTypeName",
                table: "MessageReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_ReportTypeName",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "ReportTypeName",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "ReportTypeName",
                table: "MessageReports");

            migrationBuilder.DropColumn(
                name: "ReportTypeName",
                table: "CommentReports");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ReportTypes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ReportTypeId",
                table: "PostReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReportTypeId",
                table: "MessageReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReportTypeId",
                table: "CommentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ReportTypeId",
                table: "PostReports",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReports_ReportTypeId",
                table: "MessageReports",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReportTypeId",
                table: "CommentReports",
                column: "ReportTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeId",
                table: "CommentReports",
                column: "ReportTypeId",
                principalTable: "ReportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReports_ReportTypes_ReportTypeId",
                table: "MessageReports",
                column: "ReportTypeId",
                principalTable: "ReportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReports_ReportTypes_ReportTypeId",
                table: "PostReports",
                column: "ReportTypeId",
                principalTable: "ReportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReports_ReportTypes_ReportTypeId",
                table: "MessageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReports_ReportTypes_ReportTypeId",
                table: "PostReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes");

            migrationBuilder.DropIndex(
                name: "IX_PostReports_ReportTypeId",
                table: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_MessageReports_ReportTypeId",
                table: "MessageReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_ReportTypeId",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ReportTypes");

            migrationBuilder.DropColumn(
                name: "ReportTypeId",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "ReportTypeId",
                table: "MessageReports");

            migrationBuilder.DropColumn(
                name: "ReportTypeId",
                table: "CommentReports");

            migrationBuilder.AddColumn<string>(
                name: "ReportTypeName",
                table: "PostReports",
                type: "nvarchar(64)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReportTypeName",
                table: "MessageReports",
                type: "nvarchar(64)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReportTypeName",
                table: "CommentReports",
                type: "nvarchar(64)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ReportTypeName",
                table: "PostReports",
                column: "ReportTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReports_ReportTypeName",
                table: "MessageReports",
                column: "ReportTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReportTypeName",
                table: "CommentReports",
                column: "ReportTypeName");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_ReportTypes_ReportTypeName",
                table: "CommentReports",
                column: "ReportTypeName",
                principalTable: "ReportTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReports_ReportTypes_ReportTypeName",
                table: "MessageReports",
                column: "ReportTypeName",
                principalTable: "ReportTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReports_ReportTypes_ReportTypeName",
                table: "PostReports",
                column: "ReportTypeName",
                principalTable: "ReportTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
