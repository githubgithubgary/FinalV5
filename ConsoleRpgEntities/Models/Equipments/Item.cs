using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Rooms;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleRpgEntities.Models.Equipments
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Weight { get; set; }
        //public int? InventoryId { get; set; }
        public int Value { get; set; }
        public int? PlayerId { get; set; }
        public virtual Player? Player { get; set; }
        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }
        public bool Enchanted { get; set; }
        //
        public Item()
        {
        }
        public Item(string name, string type, int attack, int defense, int value, decimal weight, int playerid, int roomid)
        {

        }
        //

    }

}
