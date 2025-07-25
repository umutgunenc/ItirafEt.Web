using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItirafEt.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAED2ATLdR3ZV/GbRKgMX8CnBTh6eN737Th/HT2GGvgGOLlzEQ50HcF8GT5XPRB2scoA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDGkeNBPkIC6dpfiEZADjVlY4moqDLEdnjPJsoYwJisCORLAorXXMHStspf6Yf4KtA==");
        }
    }
}
