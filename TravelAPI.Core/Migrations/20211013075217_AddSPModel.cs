using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class AddSPModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EpisodeUrl",
                table: "EpisodesRelatedForRecentlyModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SuperStarModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewerID = table.Column<int>(type: "int", nullable: false),
                    InterviewerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountViews = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperStarModel", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuperStarModel");

            migrationBuilder.DropColumn(
                name: "EpisodeUrl",
                table: "EpisodesRelatedForRecentlyModel");
        }
    }
}
