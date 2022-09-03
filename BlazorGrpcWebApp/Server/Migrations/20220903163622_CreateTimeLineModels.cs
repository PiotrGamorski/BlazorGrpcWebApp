using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGrpcWebApp.Server.Migrations
{
    public partial class CreateTimeLineModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LastActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLastActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LastActivityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLastActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLastActivities_LastActivities_LastActivityId",
                        column: x => x.LastActivityId,
                        principalTable: "LastActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLastActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLastActivities_LastActivityId",
                table: "UserLastActivities",
                column: "LastActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastActivities_UserId",
                table: "UserLastActivities",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLastActivities");

            migrationBuilder.DropTable(
                name: "LastActivities");
        }
    }
}
