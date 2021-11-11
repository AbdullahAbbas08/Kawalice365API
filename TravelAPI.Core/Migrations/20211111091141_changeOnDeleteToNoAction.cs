using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class changeOnDeleteToNoAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_ProgramTypes",
                table: "Programs");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_ProgramTypes",
                table: "Programs",
                column: "ProgramTypeID",
                principalTable: "ProgramTypes",
                principalColumn: "ProgramTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_ProgramTypes",
                table: "Programs");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_ProgramTypes",
                table: "Programs",
                column: "ProgramTypeID",
                principalTable: "ProgramTypes",
                principalColumn: "ProgramTypeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
