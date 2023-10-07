using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hololive.Migrations
{
    /// <inheritdoc />
    public partial class addNewColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerAge",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerDOB",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustomerAge",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustomerDOB",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "AspNetUsers");
        }
    }
}
