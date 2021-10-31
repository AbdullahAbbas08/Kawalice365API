using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class removeCollectionProgram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Category_CategoryId1",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_CategoryId1",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Programs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "Programs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programs_CategoryId1",
                table: "Programs",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Category_CategoryId1",
                table: "Programs",
                column: "CategoryId1",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
