using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class addimgprogramtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProgramTypeImgPath",
                table: "ProgramTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgramTypeOrder",
                table: "ProgramTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgramTypeImgPath",
                table: "ProgramTypes");

            migrationBuilder.DropColumn(
                name: "ProgramTypeOrder",
                table: "ProgramTypes");
        }
    }
}
