using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportExplanation",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "ReportExplanation",
                table: "CommentReactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportExplanation",
                table: "MessageReactions",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportExplanation",
                table: "CommentReactions",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);
        }
    }
}
