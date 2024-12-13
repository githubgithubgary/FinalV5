using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleRpgEntities.Models.Equipments;

public class Equipment
{
    public int Id { get; set; }

    // SQL Server enforces cascading delete rules strictly
    // so Entity Framework will assume DeleteBehavior.Cascade for relationships
    public int? WeaponId { get; set; }  // Nullable to avoid cascade issues
    public int? ArmorId { get; set; }   // Nullable to avoid cascade issues
    public int? PotionId { get; set; }  // Nullable to avoid cascade issues
    public int? AccessoryId { get; set; }   // Nullable to avoid cascade issues

    // Navigation properties
    [ForeignKey("WeaponId")]
    public virtual Item Weapon { get; set; }

    [ForeignKey("ArmorId")]
    public virtual Item Armor { get; set; }

    // Navigation properties
    [ForeignKey("PotionId")]
    public virtual Item Potion { get; set; }

    [ForeignKey("AccessoryId")]
    public virtual Item Accessory { get; set; }
    public void EquipItem(Item item)
    {
        if (item.Type == "Weapon")
        {
            Weapon = item;
        }
        else if (item.Type == "Armor")
        {
            Armor = item;
        }
        else if (item.Type == "Potion")
        {
            Potion = item;
        }
        else if (item.Type == "Accessory")
        {
            Accessory = item;
        }
    }
}
