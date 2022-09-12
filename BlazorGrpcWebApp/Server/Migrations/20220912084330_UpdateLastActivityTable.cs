using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGrpcWebApp.Server.Migrations
{
    public partial class UpdateLastActivityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpponentName",
                table: "UserLastActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserBananasGained",
                table: "UserLastActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserBananasSpent",
                table: "UserLastActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserBananasTotal",
                table: "UserLastActivities",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpponentName",
                table: "UserLastActivities");

            migrationBuilder.DropColumn(
                name: "UserBananasGained",
                table: "UserLastActivities");

            migrationBuilder.DropColumn(
                name: "UserBananasSpent",
                table: "UserLastActivities");

            migrationBuilder.DropColumn(
                name: "UserBananasTotal",
                table: "UserLastActivities");
        }
    }
}
