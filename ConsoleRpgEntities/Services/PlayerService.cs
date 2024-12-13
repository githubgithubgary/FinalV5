using ConsoleRpgEntities.Migrations;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;
using ConsoleRpgEntities.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Xml.Linq;

public class PlayerService
{
    private readonly IOutputService _outputService;
    private readonly AbilityService _abilityService;

    public PlayerService(IOutputService outputService, AbilityService abilityService)
    {
        _outputService = outputService;
        _abilityService = abilityService;
    }

    public void Attack(IPlayer player, ITargetable target)
    {
        if (player.Equipment?.Weapon == null)
        {
            _outputService.WriteLine($"{player.Name} has no weapon equipped!");
            return;
        }

        _outputService.WriteLine($"{player.Name} attacks {target.Name} with a {player.Equipment.Weapon.Name} dealing {player.Equipment.Weapon.Attack} damage!");
        target.Health -= player.Equipment.Weapon.Attack;
        _outputService.WriteLine($"{target.Name} has {target.Health} health remaining.");
    }

    public void UseAbility(IPlayer player, IAbility ability, ITargetable target)
    {
        if (player.Abilities?.Contains(ability) == true)
        {
            _abilityService.Activate(ability, player, target);
        }
        else
        {
            _outputService.WriteLine($"{player.Name} does not have the ability {ability.Name}!");
        }
    }

    public void EquipItemFromInventory(IPlayer player, Item item)
    {
        //if (player.Inventory?.Items.Contains(item) == true)
        //{
        //    player.Equipment?.EquipItem(item);
        //}
        //else
        //{
        //    _outputService.WriteLine($"{player.Name} does not have the item {item.Name} in their inventory!");
        //}
    }
    public void EditPlayerHealth(IPlayer player, int value)
    {
        if (player != null)
        {
            if (player is Player p)
            {
                int hp = p.Health;
                p.Health = p.Health + value;
                switch (value)
                {
                    case > 0:
                        _outputService.WriteLine($"Sire, your health increased from {hp} to {p.Health}, don't let this go to your head.");
                        break;
                    case < 0:
                        _outputService.WriteLine($"Sire, your health decreased from {hp} to {p.Health}, you may want to rest a bit longer.");
                        break;
                    default:
                        _outputService.WriteLine("Sire, I am not sure that really helped in anyway.");
                        break;
                }
            }
        }
        else
        {
            _outputService.WriteLine($"No player to update health!");
        }
    }
    public bool UpdatePlayerExperience(IPlayer player, int value)
    {
        if (player != null)
        {
            if (player is Player p)
            {
                int exp = p.Experience;
                p.Experience = p.Experience + value;
                switch (value)
                {
                    case > 0:
                        _outputService.WriteLine($"Great power comes with great responsibility, exp. increased from {exp} to {p.Experience}.");
                        return true;
                    case < 0:
                        _outputService.WriteLine($"Wah-Wah Whaaaaa, exp decreased from {exp} to {p.Experience}.");
                        return true;
                    default:
                        _outputService.WriteLine("Sire, I am not sure that really helped in anyway.");
                        return false;
                }
            }
            return false;
        }
        else
        {
            _outputService.WriteLine($"No player to update experience!");
            return false;
        }
    }
    public List<int> CurrentEquipment(IPlayer player, DbSet<Item> items)
    {
        _outputService.WriteLine($"Armed with the following items:");

        List<int> activeItemIds = new List<int>();

        if (player is Player pc)
        {
            Dictionary<int, string> tmp = new();
            if (pc.Equipment.WeaponId != null)
                tmp.Add((int)pc.Equipment.WeaponId, pc.Equipment.Weapon.Name);
            if (pc.Equipment.ArmorId != null)
                tmp.Add((int)pc.Equipment.ArmorId, pc.Equipment.Armor.Name);
            if (pc.Equipment.AccessoryId != null)
                tmp.Add((int)pc.Equipment.AccessoryId, pc.Equipment.Accessory.Name);
            if (pc.Equipment.PotionId != null)
                tmp.Add((int)pc.Equipment.PotionId, pc.Equipment.Potion.Name);

            Item item = null;

            if (tmp.Count > 0)
            {
                foreach (KeyValuePair<int, string> entry in tmp)
                {
                    item = items.Where(x => x.Id == entry.Key).FirstOrDefault();
                    _outputService.WriteLine($"{entry.Value} with {item.Defense} defense points and {item.Attack} attack points.");
                    activeItemIds.Add(entry.Key);
                }
            }
            else
            {
                _outputService.WriteLine($"Seriously my liege, no armor, no weapon. Going to rough if with your bare hands.");
            }
        }
        return activeItemIds;
    }
    public void AddPlayer(DbSet<Player> players, string name, int hp, int exppt)
    {
        Player new_player = new();
        new_player.Name = name;
        new_player.Health = hp;
        new_player.Experience = exppt;
        new_player.RoomId = 0;
        new_player.Active = false;

        players.Add(new_player);

        return;
    }
    public bool RemovePlayer(IPlayer player, DbSet<Room> rooms, DbSet<Item> items, DbSet<Equipment> equipments, DbSet<Player> players)
    {
        if (player is Player pc)  //  Did not have to do this but though I would
        {
            var i = items.Where(i => i.Id == player.Id).ToList();
            if (i.Count > 0)
            {
                foreach (var item in i)
                { 
                    item.PlayerId = null;
                }

                var e = equipments.Where(e => e.Id == pc.EquipmentId).ToList();
                if (e.Count > 0)
                {
                    foreach (var e2 in e)
                    {
                        equipments.Remove(e2);
                    }
                }
                var pc2 = players.Where(p => p.Id == player.Id).ToList();
                if (pc2.Count > 0)
                {
                    foreach (var p in pc2)
                    {
                        p.EquipmentId = null;
                        players.Remove(p);
                    }
                }
                return true;
            }
            else
            {
                // Something is really wrong if I get here
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
