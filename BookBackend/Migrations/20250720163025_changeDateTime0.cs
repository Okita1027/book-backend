using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_backend.Migrations
{
    /// <inheritdoc />
    public partial class changeDateTime0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCategory_Books_BookId",
                table: "BookCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCategory_Categories_CategoryId",
                table: "BookCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookCategory",
                table: "BookCategory");

            migrationBuilder.RenameTable(
                name: "BookCategory",
                newName: "bookcategory");

            migrationBuilder.RenameIndex(
                name: "IX_BookCategory_CategoryId",
                table: "bookcategory",
                newName: "IX_bookcategory_CategoryId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDate",
                table: "Loans",
                type: "datetime(0)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoanDate",
                table: "Loans",
                type: "datetime(0)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Loans",
                type: "datetime(0)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidDate",
                table: "Fines",
                type: "datetime(0)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishedDate",
                table: "Books",
                type: "datetime(0)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Authors",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bookcategory",
                table: "bookcategory",
                columns: new[] { "BookId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_bookcategory_Books_BookId",
                table: "bookcategory",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bookcategory_Categories_CategoryId",
                table: "bookcategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookcategory_Books_BookId",
                table: "bookcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_bookcategory_Categories_CategoryId",
                table: "bookcategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bookcategory",
                table: "bookcategory");

            migrationBuilder.RenameTable(
                name: "bookcategory",
                newName: "BookCategory");

            migrationBuilder.RenameIndex(
                name: "IX_bookcategory_CategoryId",
                table: "BookCategory",
                newName: "IX_BookCategory_CategoryId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDate",
                table: "Loans",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoanDate",
                table: "Loans",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Loans",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidDate",
                table: "Fines",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishedDate",
                table: "Books",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(0)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Authors",
                type: "datetime(0)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookCategory",
                table: "BookCategory",
                columns: new[] { "BookId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookCategory_Books_BookId",
                table: "BookCategory",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCategory_Categories_CategoryId",
                table: "BookCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
