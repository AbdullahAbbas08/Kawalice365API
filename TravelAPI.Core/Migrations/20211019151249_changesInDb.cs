using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class changesInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Sliders_episodeIdFk",
                table: "Episodes");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_episodeIdFk",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "episodeIdFk",
                table: "Episodes");

            migrationBuilder.RenameColumn(
                name: "episodeIdFk",
                table: "Sliders",
                newName: "SliderViews");

            migrationBuilder.AddColumn<string>(
                name: "InterviewerDescription",
                table: "SuperStarModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterviewerPicture",
                table: "SuperStarModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramIDFk",
                table: "Sliders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SliderTitle",
                table: "Sliders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramViews",
                table: "Sessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramTypeViews",
                table: "ProgramTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProgramIDFk",
                table: "Programs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramViews",
                table: "Programs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryViews",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Sliders_ProgramIDFk",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_ProgramIDFk",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "InterviewerDescription",
                table: "SuperStarModel");

            migrationBuilder.DropColumn(
                name: "InterviewerPicture",
                table: "SuperStarModel");

            migrationBuilder.DropColumn(
                name: "ProgramIDFk",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "SliderTitle",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "ProgramViews",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ProgramTypeViews",
                table: "ProgramTypes");

            migrationBuilder.DropColumn(
                name: "ProgramIDFk",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ProgramViews",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "CategoryViews",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "SliderViews",
                table: "Sliders",
                newName: "episodeIdFk");

            migrationBuilder.AddColumn<int>(
                name: "episodeIdFk",
                table: "Episodes",
                type: "int",
                nullable: true);

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
    }
}
