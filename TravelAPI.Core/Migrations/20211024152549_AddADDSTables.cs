using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BalarinaAPI.Core.Migrations
{
    public partial class AddADDSTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "AspNetUsers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ADSTYLES",
                columns: table => new
                {
                    ADStyleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ADStyleTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ADWidth = table.Column<float>(type: "real", nullable: false),
                    ADHeight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADSTYLES", x => x.ADStyleId);
                });

            migrationBuilder.CreateTable(
                name: "ADTARGETS",
                columns: table => new
                {
                    ADTargetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ADTargetTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ADTargetType = table.Column<int>(type: "int", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADTARGETS", x => x.ADTargetID);
                });

            migrationBuilder.CreateTable(
                name: "ADPLACEHOLDER",
                columns: table => new
                {
                    ADPlaceholderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdStyleID = table.Column<int>(type: "int", nullable: false),
                    AdTargetId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ADStylesADStyleId = table.Column<int>(type: "int", nullable: true),
                    ADTargetsADTargetID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADPLACEHOLDER", x => x.ADPlaceholderID);
                    table.ForeignKey(
                        name: "FK_ADPLACEHOLDER_ADSTYLES_ADStylesADStyleId",
                        column: x => x.ADStylesADStyleId,
                        principalTable: "ADSTYLES",
                        principalColumn: "ADStyleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ADPLACEHOLDER_ADTARGETS_ADTargetsADTargetID",
                        column: x => x.ADTargetsADTargetID,
                        principalTable: "ADTARGETS",
                        principalColumn: "ADTargetID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ADS",
                columns: table => new
                {
                    AdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    PlaceHolderID = table.Column<int>(type: "int", nullable: false),
                    PublishStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ADPlaceholderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADS", x => x.AdId);
                    table.ForeignKey(
                        name: "FK_ADS_ADPLACEHOLDER_ADPlaceholderID",
                        column: x => x.ADPlaceholderID,
                        principalTable: "ADPLACEHOLDER",
                        principalColumn: "ADPlaceholderID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ADPLACEHOLDER_ADStylesADStyleId",
                table: "ADPLACEHOLDER",
                column: "ADStylesADStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_ADPLACEHOLDER_ADTargetsADTargetID",
                table: "ADPLACEHOLDER",
                column: "ADTargetsADTargetID");

            migrationBuilder.CreateIndex(
                name: "IX_ADS_ADPlaceholderID",
                table: "ADS",
                column: "ADPlaceholderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADS");

            migrationBuilder.DropTable(
                name: "ADPLACEHOLDER");

            migrationBuilder.DropTable(
                name: "ADSTYLES");

            migrationBuilder.DropTable(
                name: "ADTARGETS");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "AspNetUsers");
        }
    }
}
