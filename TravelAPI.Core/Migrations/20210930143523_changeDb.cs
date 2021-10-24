using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class changeDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Programs",
                table: "Episodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Keywords_Episodes",
                table: "Episodes_Keywords");

            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Keywords_Keywords",
                table: "Episodes_Keywords");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_ProgramID",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "ProgramID",
                table: "Episodes");

            migrationBuilder.AlterColumn<bool>(
                name: "ProgramVisible",
                table: "Programs",
                type: "bit",
                nullable: false,
                defaultValueSql: "((1))",
                comment: "(0) non-Visible - (1) Visible",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "InstgramUrl",
                table: "Interviewers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "LikeRate",
                table: "Episodes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DislikeRate",
                table: "Episodes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "sessionID",
                table: "Episodes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProgramID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionID);
                    table.ForeignKey(
                        name: "FK_Sessions_Programs",
                        column: x => x.ProgramID,
                        principalTable: "Programs",
                        principalColumn: "ProgramID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_sessionID",
                table: "Episodes",
                column: "sessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ProgramID",
                table: "Sessions",
                column: "ProgramID");

            migrationBuilder.AddForeignKey(
                name: "Episodes_Sessions_FK",
                table: "Episodes",
                column: "sessionID",
                principalTable: "Sessions",
                principalColumn: "SessionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Keywords_Episodes",
                table: "Episodes_Keywords",
                column: "EpisodeID",
                principalTable: "Episodes",
                principalColumn: "EpisodeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Keywords_Keywords",
                table: "Episodes_Keywords",
                column: "KeywordID",
                principalTable: "Keywords",
                principalColumn: "KeywordID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Episodes_Sessions_FK",
                table: "Episodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Keywords_Episodes",
                table: "Episodes_Keywords");

            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Keywords_Keywords",
                table: "Episodes_Keywords");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_sessionID",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "sessionID",
                table: "Episodes");

            migrationBuilder.AlterColumn<bool>(
                name: "ProgramVisible",
                table: "Programs",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "((1))",
                oldComment: "(0) non-Visible - (1) Visible");

            migrationBuilder.AlterColumn<string>(
                name: "InstgramUrl",
                table: "Interviewers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LikeRate",
                table: "Episodes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DislikeRate",
                table: "Episodes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramID",
                table: "Episodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_ProgramID",
                table: "Episodes",
                column: "ProgramID");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Programs",
                table: "Episodes",
                column: "ProgramID",
                principalTable: "Programs",
                principalColumn: "ProgramID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Keywords_Episodes",
                table: "Episodes_Keywords",
                column: "EpisodeID",
                principalTable: "Episodes",
                principalColumn: "EpisodeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Keywords_Keywords",
                table: "Episodes_Keywords",
                column: "KeywordID",
                principalTable: "Keywords",
                principalColumn: "KeywordID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
