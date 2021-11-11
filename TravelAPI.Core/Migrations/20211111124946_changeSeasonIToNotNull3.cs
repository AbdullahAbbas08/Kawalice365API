using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class changeSeasonIToNotNull3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions");

            migrationBuilder.AlterColumn<int>(
                name: "ProgramID",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions",
                column: "ProgramID",
                principalTable: "Programs",
                principalColumn: "ProgramID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions");

            migrationBuilder.AlterColumn<int>(
                name: "ProgramID",
                table: "Sessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions",
                column: "ProgramID",
                principalTable: "Programs",
                principalColumn: "ProgramID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
