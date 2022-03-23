using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGrpcWebApp.Server.Migrations
{
    public partial class CreateBattleLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BattleLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BattleId = table.Column<int>(type: "int", nullable: false),
                    AttackerId = table.Column<int>(type: "int", nullable: false),
                    OpponentId = table.Column<int>(type: "int", nullable: false),
                    Log = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleLogs_Battles_BattleId",
                        column: x => x.BattleId,
                        principalTable: "Battles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattleLogs_Users_AttackerId",
                        column: x => x.AttackerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BattleLogs_Users_OpponentId",
                        column: x => x.OpponentId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattleLogs_AttackerId",
                table: "BattleLogs",
                column: "AttackerId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleLogs_BattleId",
                table: "BattleLogs",
                column: "BattleId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleLogs_OpponentId",
                table: "BattleLogs",
                column: "OpponentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleLogs");
        }
    }
}
