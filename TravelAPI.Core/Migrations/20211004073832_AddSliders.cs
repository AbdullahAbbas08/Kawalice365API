using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class AddSliders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "episodeIdFk",
                table: "Episodes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    SliderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SliderImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SliderOrder = table.Column<int>(type: "int", nullable: false),
                    episodeIdFk = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.SliderId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_episodeIdFk",
                table: "Episodes",
                column: "episodeIdFk");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Sliders_episodeIdFk",
                table: "Episodes",
                column: "episodeIdFk",
                principalTable: "Sliders",
                principalColumn: "SliderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Sliders_episodeIdFk",
                table: "Episodes");

            migrationBuilder.DropTable(
                name: "Sliders");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_episodeIdFk",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "episodeIdFk",
                table: "Episodes");
        }
    }
}
