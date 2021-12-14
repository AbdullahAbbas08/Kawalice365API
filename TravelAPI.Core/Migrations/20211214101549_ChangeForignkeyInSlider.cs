using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class ChangeForignkeyInSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Sliders_ProgramIDFk",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_ProgramIDFk",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ProgramIDFk",
                table: "Programs");

            migrationBuilder.RenameColumn(
                name: "ProgramIDFk",
                table: "Sliders",
                newName: "EpisodeID");

            migrationBuilder.AddColumn<int>(
                name: "SeasonIndex",
                table: "EpisodesRelatedForRecentlyModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EpisodeID1",
                table: "Episodes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_EpisodeID1",
                table: "Episodes",
                column: "EpisodeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Sliders_EpisodeID1",
                table: "Episodes",
                column: "EpisodeID1",
                principalTable: "Sliders",
                principalColumn: "SliderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Sliders_EpisodeID1",
                table: "Episodes");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_EpisodeID1",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "SeasonIndex",
                table: "EpisodesRelatedForRecentlyModel");

            migrationBuilder.DropColumn(
                name: "EpisodeID1",
                table: "Episodes");

            migrationBuilder.RenameColumn(
                name: "EpisodeID",
                table: "Sliders",
                newName: "ProgramIDFk");

            migrationBuilder.AddColumn<int>(
                name: "ProgramIDFk",
                table: "Programs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ProgramIDFk",
                table: "Programs",
                column: "ProgramIDFk");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Sliders_ProgramIDFk",
                table: "Programs",
                column: "ProgramIDFk",
                principalTable: "Sliders",
                principalColumn: "SliderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
