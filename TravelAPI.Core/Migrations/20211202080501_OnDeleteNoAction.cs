using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class OnDeleteNoAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_categories_CategoryID",
                table: "Programs");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Interviewers",
                table: "Programs");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Categories",
                table: "Programs",
                column: "CategoryID",
                principalTable: "categories",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Interviewers",
                table: "Programs",
                column: "InterviewerID",
                principalTable: "Interviewers",
                principalColumn: "InterviewerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Categories",
                table: "Programs");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Interviewers",
                table: "Programs");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_categories_CategoryID",
                table: "Programs",
                column: "CategoryID",
                principalTable: "categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Interviewers",
                table: "Programs",
                column: "InterviewerID",
                principalTable: "Interviewers",
                principalColumn: "InterviewerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
