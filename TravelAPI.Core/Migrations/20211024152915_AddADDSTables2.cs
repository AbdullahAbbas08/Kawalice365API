using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class AddADDSTables2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientID",
                table: "ADS",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ADS_ClientID",
                table: "ADS",
                column: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_ADS_AspNetUsers_ClientID",
                table: "ADS",
                column: "ClientID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ADS_AspNetUsers_ClientID",
                table: "ADS");

            migrationBuilder.DropIndex(
                name: "IX_ADS_ClientID",
                table: "ADS");

            migrationBuilder.DropColumn(
                name: "ClientID",
                table: "ADS");
        }
    }
}
