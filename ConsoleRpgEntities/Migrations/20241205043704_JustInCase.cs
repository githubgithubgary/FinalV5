using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    public partial class JustInCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessoryId",
                table: "Equipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PotionId",
                table: "Equipments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_AccessoryId",
                table: "Equipments",
                column: "AccessoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_PotionId",
                table: "Equipments",
                column: "PotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Items_AccessoryId",
                table: "Equipments",
                column: "AccessoryId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Items_PotionId",
                table: "Equipments",
                column: "PotionId",
                principalTable: "Items",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Items_AccessoryId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Items_PotionId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_AccessoryId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_PotionId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "AccessoryId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "PotionId",
                table: "Equipments");
        }
    }
}
