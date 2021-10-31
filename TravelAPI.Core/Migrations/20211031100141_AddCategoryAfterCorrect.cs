using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class AddCategoryAfterCorrect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Categories_CategoryID",
                table: "Programs");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "categories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "categories",
                newName: "CategoryID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "categories",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryTitle",
                table: "categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryImg",
                table: "categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryDescription",
                table: "categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_categories_CategoryID",
                table: "Programs",
                column: "CategoryID",
                principalTable: "categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_categories_CategoryID",
                table: "Programs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.RenameTable(
                name: "categories",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryTitle",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryImg",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryDescription",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "CategoryId");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryOrder = table.Column<int>(type: "int", nullable: false),
                    CategoryTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CategoryViews = table.Column<int>(type: "int", nullable: false),
                    CategoryVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Categories_CategoryID",
                table: "Programs",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
