using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO_CRUD.Migrations
{
    /// <inheritdoc />
    public partial class DateTimeChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "Users",
                type: "datetime(0)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Authors",
                type: "datetime(0)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Authors",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)");
        }
    }
}
