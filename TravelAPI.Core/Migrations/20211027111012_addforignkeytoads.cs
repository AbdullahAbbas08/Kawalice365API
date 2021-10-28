using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class addforignkeytoads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ADS_ADPLACEHOLDER_ADPlaceholderID",
                table: "ADS");

            migrationBuilder.DropIndex(
                name: "IX_ADS_ADPlaceholderID",
                table: "ADS");

            migrationBuilder.DropColumn(
                name: "ADPlaceholderID",
                table: "ADS");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "EpisodesRelatedForRecentlyModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ADS_PlaceHolderID",
                table: "ADS",
                column: "PlaceHolderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ADS_ADPLACEHOLDER_PlaceHolderID",
                table: "ADS",
                column: "PlaceHolderID",
                principalTable: "ADPLACEHOLDER",
                principalColumn: "ADPlaceholderID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ADS_ADPLACEHOLDER_PlaceHolderID",
                table: "ADS");

            migrationBuilder.DropIndex(
                name: "IX_ADS_PlaceHolderID",
                table: "ADS");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "EpisodesRelatedForRecentlyModel");

            migrationBuilder.AddColumn<int>(
                name: "ADPlaceholderID",
                table: "ADS",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ADS_ADPlaceholderID",
                table: "ADS",
                column: "ADPlaceholderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ADS_ADPLACEHOLDER_ADPlaceholderID",
                table: "ADS",
                column: "ADPlaceholderID",
                principalTable: "ADPLACEHOLDER",
                principalColumn: "ADPlaceholderID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
