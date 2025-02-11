using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS_Assigment2.Migrations
{
    /// <inheritdoc />
    public partial class AddLastPasswordChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChange",
                table: "AspNetUsers",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHistory",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPasswordChange",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordHistory",
                table: "AspNetUsers");
        }
    }
}
