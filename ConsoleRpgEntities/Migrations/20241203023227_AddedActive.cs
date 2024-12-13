using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    public partial class AddedActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Players_PlayerId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAbilities_Players_PlayersId",
                table: "PlayerAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Equipments_EquipmentId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Rooms_RoomId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.RenameTable(
                name: "Players",
                newName: "Player");

            migrationBuilder.RenameIndex(
                name: "IX_Players_RoomId",
                table: "Player",
                newName: "IX_Player_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_EquipmentId",
                table: "Player",
                newName: "IX_Player_EquipmentId");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Player",
                type: "bit",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Player",
                table: "Player",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Player_PlayerId",
                table: "Inventory",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Player_Equipments_EquipmentId",
                table: "Player",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Player_Rooms_RoomId",
                table: "Player",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAbilities_Player_PlayersId",
                table: "PlayerAbilities",
                column: "PlayersId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Player_PlayerId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Player_Equipments_EquipmentId",
                table: "Player");

            migrationBuilder.DropForeignKey(
                name: "FK_Player_Rooms_RoomId",
                table: "Player");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAbilities_Player_PlayersId",
                table: "PlayerAbilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Player",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Player");

            migrationBuilder.RenameTable(
                name: "Player",
                newName: "Players");

            migrationBuilder.RenameIndex(
                name: "IX_Player_RoomId",
                table: "Players",
                newName: "IX_Players_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Player_EquipmentId",
                table: "Players",
                newName: "IX_Players_EquipmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Players_PlayerId",
                table: "Inventory",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAbilities_Players_PlayersId",
                table: "PlayerAbilities",
                column: "PlayersId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Equipments_EquipmentId",
                table: "Players",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Rooms_RoomId",
                table: "Players",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
