using Balarina.Utility;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace BalarinaAPI.Core.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData
                                      (
                                          table: "AspNetRoles",
                                          columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                                          values: new object[] { Guid.NewGuid().ToString(), StaticDifintions.UserManager, StaticDifintions.UserManager.ToUpper(), Guid.NewGuid().ToString() }
                                      );
            migrationBuilder.InsertData
               (
                   table: "AspNetRoles",
                   columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                   values: new object[] { Guid.NewGuid().ToString(), StaticDifintions.User, StaticDifintions.User.ToUpper(), Guid.NewGuid().ToString() }
               );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from [AspNetRoles]");
        }
    }
}
