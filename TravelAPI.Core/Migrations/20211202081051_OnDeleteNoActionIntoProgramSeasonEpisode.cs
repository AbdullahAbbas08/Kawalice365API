using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class OnDeleteNoActionIntoProgramSeasonEpisode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Episodes_Sessions_FK",
                table: "Episodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "Episodes_Sessions_FK",
                table: "Episodes",
                column: "sessionID",
                principalTable: "Sessions",
                principalColumn: "SessionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions",
                column: "ProgramID",
                principalTable: "Programs",
                principalColumn: "ProgramID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Episodes_Sessions_FK",
                table: "Episodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "Episodes_Sessions_FK",
                table: "Episodes",
                column: "sessionID",
                principalTable: "Sessions",
                principalColumn: "SessionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Programs",
                table: "Sessions",
                column: "ProgramID",
                principalTable: "Programs",
                principalColumn: "ProgramID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
