using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    public partial class AddedRoomIdToItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_RoomId",
                table: "Items",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Rooms_RoomId",
                table: "Items",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Rooms_RoomId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_RoomId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Items");
        }
    }
}
