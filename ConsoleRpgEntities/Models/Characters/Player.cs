using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;
using System.ComponentModel.Design;

namespace ConsoleRpgEntities.Models.Characters
{
    public class Player : IPlayer, ITargetable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public bool? Active { get; set; }

        // Foreign key
        public int? EquipmentId { get; set; }
        //public int? InventoryId { get; set; }

        // Navigation properties
        //public virtual Inventory Inventory { get; set;

        public virtual Equipment Equipment { get; set; }
        public virtual Room Room { get; set; }
        public virtual int? RoomId { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }
    }

}
