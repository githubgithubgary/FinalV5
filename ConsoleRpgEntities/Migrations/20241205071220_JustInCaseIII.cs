using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    public partial class JustInCaseIII : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Items_Inventory_InventoryId",
            //    table: "Items");

            //migrationBuilder.DropTable(
            //    name: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Items_InventoryId",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "Items",
                newName: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "Items",
                newName: "InventoryId");

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_InventoryId",
                table: "Items",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_PlayerId",
                table: "Inventory",
                column: "PlayerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Inventory_InventoryId",
                table: "Items",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");
        }
    }
}
