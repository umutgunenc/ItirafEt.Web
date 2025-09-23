using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Messages",
                newName: "Thumbnail");

            migrationBuilder.AddColumn<string>(
                name: "PhotoId",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Thumbnail",
                table: "Messages",
                newName: "PhotoUrl");
        }
    }
}
