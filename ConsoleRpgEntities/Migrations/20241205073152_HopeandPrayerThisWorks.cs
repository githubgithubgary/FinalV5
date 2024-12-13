using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    public partial class HopeandPrayerThisWorks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Items_PlayerId",
                table: "Items",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Player_PlayerId",
                table: "Items",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Player_PlayerId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_PlayerId",
                table: "Items");
        }
    }
}
