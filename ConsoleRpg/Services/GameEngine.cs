using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;
using ConsoleRpgEntities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.ComponentModel.Design;
//using Spectre.Console;

namespace ConsoleRpg.Services;
public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly MapManager _mapManager;
    private readonly PlayerService _playerService;
    private readonly InventoryService _inventoryService;
    private readonly OutputManager _outputManager;
    private Microsoft.EntityFrameworkCore.Metadata.Internal.Table _logTable;
    private Spectre.Console.Panel _mapPanel;

    private Player _player;
    private IMonster _goblin;

    public GameEngine(GameContext context, MenuManager menuManager, MapManager mapManager, PlayerService playerService, OutputManager outputManager, InventoryService inventoryService)
    {
        _menuManager = menuManager;
        _mapManager = mapManager;
        _playerService = playerService;
        _inventoryService = inventoryService;
        _outputManager = outputManager;
        _context = context;
        _inventoryService = inventoryService;
    }

    public void Run()
    {
        while (true)
        {
            int retval = _menuManager.ShowMainMenu();
            switch (retval)
            {
                case 1:
                    SetupGame();
                    continue;
                case 0:
                    //_outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                    Environment.Exit(0);
                    break;
                default:
                    continue;
            }
        }
    }
    private void GameLoop()
    {
        //www.geeksforgeeks.org/c-sharp-dictionary-with-examples/
//        Dictionary<string, string> NoPlayerMenu = new Dictionary<string, string>() {
//            { "1", "Add Player" }
//        };
        Dictionary<string, string> GameLoopMenu = new Dictionary<string, string>() {
            { "1", "Player Maintenance Menu" },
            { "2", "Player Action Menu" },
            { "3", "Inventory Management Menu" },
            { "4", "Administration Menu" }
            //,
        };
        Dictionary<string, string> MainMenu = null;

        while (true)
        {
            string? value;
            MainMenu = GameLoopMenu;
            var input = "0"; // Fudging something here not good but works
            foreach (KeyValuePair<string, string> element in MainMenu)
            {
                value = String.Format("{0}. {1}", element.Key, element.Value);
                _outputManager.AddLogEntry(value);
            }
            _outputManager.AddLogEntry("0. Exit");
            input = _outputManager.GetUserInput("Choose an action:");
            switch (input)
            {
                case "1":
                    if (_player == null)
                    {
                        AddPlayer();
                    }
                    else
                    {
                        PlayerMaintenanceMenu();
                    }
                    break;
                case "2":
                    PlayerActionMenu();
                    break;
                case "3":
                    InventoryMenu();
                    break;
                case "4":
                    AdminMenu();
                    break;
                case "0":
                    _outputManager.AddLogEntry("Exiting game...");
                    Environment.Exit(0);
                    break;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    break;
            }
        }
    }

    private void AttackCharacter()
    {
        if (_goblin is ITargetable targetableGoblin)
        {
            _playerService.Attack(_player, targetableGoblin);
            _playerService.UseAbility(_player, _player.Abilities.First(), targetableGoblin);
        }
    }

    private void SetupGame()
    {
        //_player = _context.Player.FirstOrDefault();
        _player = _context.Player.Where(obj => obj.Active == true).FirstOrDefault();
        //_player = _context.Players.Where(p => p.Active == true).FirstOrDefault();
        //TODO Possibly ask about this
        //  _player = _context.Players.Where(p => p.Active == true);
        //                  .Select(p => p.Id);
        if (_player != null)
        {
            _outputManager.AddLogEntry($"Sire, {_player.Name} has entered the game.");
            _mapManager.LoadInitialRoom((int)_player.RoomId);
        }
        else
        {
            _outputManager.AddLogEntry($"No active players have been setup.");
            _mapManager.LoadInitialRoom(1);
        }
        // Load map
        _mapManager.DisplayMap();

        // Load monsters into random rooms 
        LoadMonsters();

        //Load items into random rooms
        LoadItems();


        // Pause before starting the game loop
        Thread.Sleep(500);
        GameLoop();
    }

    private void LoadMonsters()
    {
        _goblin = _context.Monsters.OfType<Goblin>().FirstOrDefault();
    }
    private void LoadItems()
    {
        List<Item> items = _context.Items.Where(i => i.PlayerId == null && i.RoomId != null).ToList();
        if (items.Count > 0)
        {
            _outputManager.AddLogEntry("Items have been previously loaded, nothing to do here.");
            return;
        }
        items = _context.Items.Where(i => i.PlayerId == null && i.RoomId  == null).ToList();
        if (items.Count == 0)
        {
            _outputManager.AddLogEntry("No items let to load into the Dungeon.");
        }
        else
        {
            Random headortails = new Random();
            Random rnd_itm_cnt = new Random();
            int max_rnd_itms = 5;
            int itmcntper = 0;

            int rmcnt = _context.Rooms.Count();
            int itmcnt = items.Count()-1; // Always remove the outside area

            foreach (Room rm in _context.Rooms.Where(r => r.Id != 0).ToList())
            {
                int coinToss = headortails.Next(0, 2);
                if (coinToss == 1)
                {
                    itmcntper = rnd_itm_cnt.Next(1, max_rnd_itms);// creates a number between 1 and max number of items per room
                    for (int i = 0; i < itmcntper; i++)
                    {
                        items[i].RoomId = rm.Id;
                        items.Remove(items[i]);
                        _outputManager.AddLogEntry($"Addint item {items[i].Name} to room {rm.Name}.");
                    }
                    _outputManager.AddLogEntry($"Commit items to {rm.Name} room.");
                    _context.SaveChanges();
                }
                else
                { 
                    if (items.Count == 0)
                    {
                        _outputManager.AddLogEntry("No items more items left to load into Dungeon.");
                        break;
                    }
                }
            }
        }
    }
    private void AddPlayer()
    {
        while (true)
        {
            _outputManager.AddLogEntry("Enter new player name.");
            var input = _outputManager.GetUserInput("Please enter the name of your mighty hero/heroine, my liege or 'exit' to return to previous menu.");
            if (input != null)
            {
                if (input.ToLower() == "exit")
                {
                    _outputManager.AddLogEntry("Exit add player");
                    break;
                }
                else
                {
                    string name = input;
                    input = _outputManager.GetUserInput($"Please enter {name}'s experience points or 'exit' to return to previous menu.");
                    if (input.ToLower() == "exit")
                    {
                        _outputManager.AddLogEntry("Exit add player");
                        break;
                    }
                    else
                    {
                        int value = 0;
                        if (Int32.TryParse(input, out value) == true)
                        {
                            int hp = value;

                            input = _outputManager.GetUserInput($"Please enter {name}'s health points or 'exit' to return to previous menu.");
                            if (input.ToLower() == "exit")
                            {
                                _outputManager.AddLogEntry("Exit add player");
                                break;
                            }
                            else
                            {
                                value = 0;
                                if (Int32.TryParse(input, out value) == true)
                                {
                                    int exppt = value;
                                    _playerService.AddPlayer(_context.Player, name, hp, exppt);
                                    _context.SaveChanges();
                                    _outputManager.AddLogEntry("Player added");
                                    break;
                                }
                                else
                                {
                                    _outputManager.AddLogEntry("Invalid entry, please try again.");
                                }
                            }
                        }
                        else
                        {
                            _outputManager.AddLogEntry("Invalid entry, please try again.");
                        }
                    }
                }
            }
            else
            {
                _outputManager.AddLogEntry("Invalid entry, please try again.");
            }
        }
        _outputManager.AddLogEntry("Exit add player");
        return;
    }
    private void PlayerMaintenanceMenu()
    {
        while (true)
        {
            int input = _menuManager.ShowPlayerMaintenanceMenu();
            //var input = _outputManager.GetUserInput("Choose an action:");

            switch (input)
            {
                case 1:
                    DisplayActivePlayer();
                    break;
                case 2:
                    AddPlayer();
                    break;
                case 3:
                    EditActivePlayerMenu();
                    break;
                case 4:
                    EquipPlayer();
                    break;
                case 5:
                    ListAllPlayers();
                    break;
                case 0:
                    _outputManager.AddLogEntry("Previous Menu...");
                    //Environment.Exit(0);
                    return;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    break;
            }
        }
        return;
    }
    
    private void ListAllPlayers()
    {
        if (_context.Player.Count() == 0)
        {
            _outputManager.AddLogEntry("Sorry my liege, this campaign seems to be a bit lite on brave and might heros.");
        }
        else
        {
            int cnt = 1;
            _outputManager.AddLogEntry("List of current players is...");
            foreach (var players in _context.Player)
            {
                _outputManager.AddLogEntry($"{cnt}. {players.Name}");

                cnt = cnt + 1;
            }
            _outputManager.GetUserInput("Press any key to return to previous menu.");
        }
    }
    private void DisplayActivePlayer()
    {

        if (_player != null)
        {
            _outputManager.AddLogEntry($"My liege, the current hero stats are as follows:");
            _outputManager.AddLogEntry($"Name: {_player.Name}");
            _outputManager.AddLogEntry($"HP: {_player.Health}");
            _outputManager.AddLogEntry($"Experience: {_player.Experience}");
            if (_player.EquipmentId != null)
            {
                _outputManager.AddLogEntry("Active Equipment:");
                List<int> actEquipIds = _playerService.CurrentEquipment(_player, _context.Items);
                //Union
                //items.Except(list2)
                // UseAbility the @ sign to get rid of the error
                //stackoverflow.com/questions/1202981/select-multiple-fields-from-list-in-linq
                //List<Item> equipIds = (List<Item>)_context.Items.Where(a => a.PlayerId == _player.Id);

                //stackoverflow.com/questions/2554312/linq-how-to-query-specific-columns-and-return-a-lists

                if (actEquipIds.Count() > 0)
                {
                    _outputManager.AddLogEntry("Complete Equipment Listing:");
                    // Would display a list of the other items that are not 'Equipment' that is assigned to this playerid
                    var items = _context.Items.Where(i => i.PlayerId == _player.Id).ToList();
                    _inventoryService.ListItems(items);
                    _outputManager.GetUserInput($"Press any key to continue...");
                }
                else
                {
                    _outputManager.AddLogEntry("Sorry my liege your pockets are a bit empty at the moment.");
                }
            }
            else
            {
                _outputManager.AddLogEntry("Equipment: None");
            }

            if (_player.RoomId != null)
            {
                Room pc_rm = _context.Rooms.Where(r => r.Id == _player.RoomId).FirstOrDefault();
                if (pc_rm != null)
                {
                    _outputManager.AddLogEntry($"Current Location: {pc_rm.Name}");
                }
            }
            else
            {
                _outputManager.AddLogEntry("Current Location: Unknown");
            }
        }
        else
        {
            _outputManager.AddLogEntry("Sorry sire but I cannot seem to find an active hero/heroine at this time.");
        }
    }
    private void EditActivePlayerMenu()
    {
        while (true)
        {
            int input = _menuManager.ShowEditActivePlayerMenu();
            switch (input)
            {
                case 1:
                    SwitchActivePlayer();
                    break;
                case 2:
                    PlayerHealth();
                    break;
                case 3:
                    EditPlayerActiveItem("Armor");
                    break;
                case 4:
                    EditPlayerActiveItem("Weapon");
                    break;
                case 5:
                    EditPlayerActiveItem("Potion");
                    break;
                case 6:
                    EditPlayerActiveItem("Accessory");
                    break;
                case 0:
                    _outputManager.AddLogEntry("Previous Menu...");
                    //Environment.Exit(0);
                    return;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    break;
            }
        }
    }
    private void SwitchActivePlayer()
    {
        List<Player> inact_players = null;
        while (true)
        {
            inact_players = _context.Player.Where(obj => obj.Active == false).ToList();

            if (inact_players != null)
            {
                _outputManager.AddLogEntry($"Select a new player by entering the number next to the name:");
                int idx = 1;
                foreach (Player iap in inact_players)
                {
                    _outputManager.AddLogEntry($"{idx}. {iap.Name}");
                    idx += 1;
                }
                var value = _outputManager.GetUserInput($"Enter a number or 0 to return to previous? ");
                int choice = 0;
                if (Int32.TryParse(value, out choice) == true)
                {
                    switch (choice)
                    {
                        case 0:
                            _outputManager.AddLogEntry("Returning to previous menu...");
                            return;
                        case > 0:
                            Player new_player = _context.Player.Where(obj => obj.Name == inact_players[choice - 1].Name).FirstOrDefault();
                            if (_player != null)
                            {
                                _outputManager.AddLogEntry($"All right, if that is what you wish my liege, out with {_player.Name} and in with {new_player.Name}");
                                value = _outputManager.GetUserInput($"Yay or nay say you sire (Y or N)? ");
                                if (value.ToUpper() == "Y" || value.ToUpper() == "Yay" || value.ToUpper() == "Yes")
                                {
                                    _outputManager.AddLogEntry($"{_player.Name} vanished into to thin air.");
                                    _outputManager.AddLogEntry($"The heroic {new_player.Name} has just appeared from no where, the gods show us favor!!!");
                                    _player.Active = false;
                                    // Connect the new player to the old players room
                                    new_player.RoomId = _player.RoomId;

                                    //Update the room with the new player
                                    Room rm = _context.Rooms.FirstOrDefault(obj => obj.Id == _player.RoomId);
                                    rm.PlayerId = new_player.Id;

                                    //Remove the old player from the room
                                    _player.RoomId = 0;

                                    // Switch the active player
                                    new_player.Active = true;
                                    _player = new_player;
                                    _context.SaveChanges();
                                    break;
                                }
                                else
                                {
                                    if (value.ToUpper() == "N" || value.ToUpper() == "Nay" || value.ToUpper() == "No")
                                    {
                                        _outputManager.AddLogEntry($"Let us hope you have made a wise choice my liege.");
                                        break;
                                    }
                                    else
                                    {
                                        _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                // Switch the active player
                                new_player.Active = true;
                                _player = new_player;
                                _context.SaveChanges();
                                return;
                            }
                        default:
                            _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                            continue;
                    }
                }
                else
                {
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                }
            }
            else
            {
                _outputManager.AddLogEntry($"Sorry my liege, there are no more mighty heros to choose from, {_player.Name} is the last...");
                _outputManager.GetUserInput($"Press any key to continue...");
                return;
            }
        }
    }
    private void PlayerHealth()
    {
        while (true)
        {
            _outputManager.AddLogEntry($"Enter a value to increase or decrease the health, use a minus (-) to decrease");
            var value = _outputManager.GetUserInput($"Enter a value postive or negative value:");
            if (value != null)
            {
                int num = 0;
                if (Int32.TryParse(value, out num))
                {
                    _playerService.EditPlayerHealth(_player, num);

                    // Put it here for now just to force the udpate if I need to increase or decrease weight allowance for testing
                    _context.SaveChanges();
                    break;
                }
                else
                {
                    _outputManager.AddLogEntry("Invalid, please try again.");
                    _outputManager.GetUserInput("Any key to continue...");
                }
            }
        }
        return;
    }
    private void EditPlayerActiveItem(string thing)
    {
        List<Item> items = null;
        while (true)
        {
            items = _context.Items.Where(obj => obj.Type == thing && obj.PlayerId == _player.Id ).ToList();
            if (items.Count() != 0)
            {
                _outputManager.AddLogEntry($"Select a new item type {thing} by entering the number next to the item:");
                int idx = 1;
                foreach (Item i in items)
                {
                    _outputManager.AddLogEntry($"{idx}. {i.Name}");
                    idx += 1;
                }
                var value = _outputManager.GetUserInput($"Enter a number or 0 to return to previous? ");
                int choice = 0;
                if (Int32.TryParse(value, out choice) == true)
                {
                    switch (choice)
                    {
                        case 0:
                            _outputManager.AddLogEntry("Returning to previous menu...");
                            return;
                        case > 0:
                            //TODO Implement switch out active items
                            _outputManager.AddLogEntry("Here");
                            continue;
                        default:
                            _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                            continue;
                    }
                }
                else
                {
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                }
            }
            else
            {
                _outputManager.AddLogEntry($"Sorry my liege, there are no more items of type {thing} to choose.");
                _outputManager.GetUserInput($"Press any key to continue...");
                return;
            }
        }
    }
    private void PlayerActionMenu()
    {
        while (true)
        {
            int input = _menuManager.ShowPlayerActionMenu();
            switch (input)
            {
                case 1:
                    ExploreDungeon();
                    break;
                case 0:
                    _outputManager.AddLogEntry("Previous Menu...");
                    //Environment.Exit(0);
                    return;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    break;
            }
        }
    }
    private void ExploreDungeon()
    {
        if (_player.RoomId != null)
        {
            _outputManager.AddLogEntry($"{_player.Name} is currently in the {_player.Room.Name}");
            while (true)
            {
                string choice = _outputManager.GetUserInput("Pick a direction to move: N,E,S,W or X to exit the dungeon?");
                if (choice != null)
                {
                    string direction = choice.ToUpper();
                    switch (direction)
                    {
                        case "N": case "E": case "S": case "W":
                            Navigate(direction);
                            _mapManager.DisplayMap();
                            _context.SaveChanges();
                            break;
                        case "X":
                            _outputManager.AddLogEntry("Returning to previous menu...");
                            return;
                        default:
                            _outputManager.AddLogEntry("Invalid selection, please try again.");
                            continue;
                    }
                }
                else
                {
                    _outputManager.AddLogEntry("Invalid selection, please try again.");
                }
            }
        }
        else
        {
            _outputManager.AddLogEntry($"Sorry my leige, I cannot find a current location for {_player.Name}.");
        }
    }
    private void Navigate(string direction)
    {
        MapManager map = _mapManager; // Lets make a copy of the map manager
        Room curr_room = _context.Rooms.Where(obj => obj.Id == _player.RoomId).FirstOrDefault(); // Find out where player is
        if (curr_room != null)
        {
            Room rm = null;
            switch (direction)
            {
                case "N":
                    if (curr_room.NorthId != null)
                    {
                        _mapManager.UpdateCurrentRoom((int)curr_room.NorthId);
                        _player.RoomId = curr_room.NorthId;
                        curr_room.PlayerId = null;
                        rm = _context.Rooms.Where(obj => obj.Id == curr_room.NorthId).FirstOrDefault();
                        _outputManager.AddLogEntry($"Entering {rm.Name}");
                        rm.PlayerId = _player.Id;
                    }
                    else 
                    {
                        _outputManager.AddLogEntry("Looks like a dead end to the North my liege, perhaps a new direction.");
                    }
                    break;
                case "E":
                    if (curr_room.EastId != null)
                    {
                        _mapManager.UpdateCurrentRoom((int)curr_room.EastId);
                        _player.RoomId = curr_room.EastId;
                        curr_room.PlayerId = null;
                        rm = _context.Rooms.Where(obj => obj.Id == curr_room.EastId).FirstOrDefault();
                        _outputManager.AddLogEntry($"Entering {rm.Name}");
                        rm.PlayerId = _player.Id;
                    }
                    else
                    {
                        _outputManager.AddLogEntry("Looks like a dead end to the East my liege, perhaps a new direction.");
                    }
                    break;
                case "S":
                    if (curr_room.SouthId != null)
                    {
                        _mapManager.UpdateCurrentRoom((int)curr_room.SouthId);
                        _player.RoomId = curr_room.SouthId;
                        curr_room.PlayerId = null;
                        rm = _context.Rooms.Where(obj => obj.Id == curr_room.SouthId).FirstOrDefault();
                        _outputManager.AddLogEntry($"Entering {rm.Name}");
                        rm.PlayerId = _player.Id;
                    }
                    else
                    {
                        _outputManager.AddLogEntry("Looks like a dead end to the South my liege, perhaps a new direction.");
                    }
                    break;
                case "W":
                    if (curr_room.WestId != null)
                    {
                        _mapManager.UpdateCurrentRoom((int)curr_room.WestId);
                        _player.RoomId = curr_room.WestId;
                        curr_room.PlayerId = null;
                        rm = _context.Rooms.Where(obj => obj.Id == curr_room.WestId).FirstOrDefault();
                        _outputManager.AddLogEntry($"Entering {rm.Name}");
                        rm.PlayerId = _player.Id;
                    }
                    else
                    {
                        _outputManager.AddLogEntry("Looks like a dead end to the West my liege, perhaps a new direction.");
                    }
                    break ;
                default:
                    break;
            } 
            if (rm != null)
            {
                List<Item> items = _context.Items.Where(i => i.RoomId == rm.Id).ToList();
                if (items.Count > 0)
                {
                    _outputManager.AddLogEntry("We struck pay dirt sire the room has the following...");
                    _inventoryService.ListItems(items);
                    PickItUp(items);
                }
                else
                {
                    if (rm.Id == 0)
                    {
                        _outputManager.AddLogEntry($"I'm all about fresh air my liege, perhaps we turn back inside and find more precious, I mean loot?");
                    }
                    else
                    {
                        _outputManager.AddLogEntry($"The room is barren my liege, lets move on and look for more goodies.");
                    }
                }
            }
        }
    }
    private void AdminMenu()
    {
        while (true)
        {
            int input = _menuManager.ShowAdminMenu();
            switch (input)
            {
                case 1:
                    RemovePlayer();
                    break;
                case 2:
                    UpdatePlayerExperience();
                    break;
                case 3:
                    FindItems();
                    break;
                case 4:
                    FindOccupiedRooms();
                    break;
                case 5:
                     ClearItemsFromRooms();
                    break;
                case 6:
                    ListRoomContents();
                    break;
                case 0:
                    _outputManager.AddLogEntry("Previous Menu...");
                    return;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    break;
            }
        }
        return;
    }
    private void ClearItemsFromRooms()
    {
        while (true)
        {
            var input = _outputManager.GetUserInput($"Clear 'one' room or 'all' rooms, exit to return to previous menu?");
            if (input != null)
            {
                switch(input.ToLower())
                {
                    case "exit":
                        _outputManager.AddLogEntry("Previous Menu...");
                        return;
                    case "all":
                        ClearRooms(100);
                        continue;
                    case "one":
                        ClearRooms(1);
                        continue;
                    default:
                        _outputManager.AddLogEntry("Not sure that I got that, so lets try again shall we.");
                        continue;
                }
            }
            else
            {
                _outputManager.AddLogEntry("Not sure that I got that, so lets try again shall we.");
            }
        }
    }
    private void ClearRooms(int howmany)
    {
        if (howmany == 100)
        {
            var value = _outputManager.GetUserInput($"Clear all rooms of all items, yay or nay say you sire (Y or N)? ");
            if (value.ToUpper() == "Y" || value.ToUpper() == "Yay" || value.ToUpper() == "Yes")
            {
                var items = _context.Items.Where(i => i.PlayerId == null && i.RoomId != null).ToList();
                if (items.Count == 0)
                {
                    _outputManager.AddLogEntry("There were no items found in any rooms.");
                }
                else
                {
                    _inventoryService.ClearItems(items);
                    _context.SaveChanges();
                }
            }
            else
            {
                _outputManager.AddLogEntry("Previous Menu...");
            }
        }
        else
        {
            while (true)
            {
                var input = _outputManager.GetUserInput($"Which room would you like to clear out or exit to return to previous menu?");
                if (input != null)
                {
                    if (input == "exit")
                    {
                        _outputManager.AddLogEntry("Previous Menu...");
                        return;
                    }
                    else
                    {
                        string name = (string)input;
                        List<Room> rmtmp = _context.Rooms.Where(r => r.Name == name).ToList();
                        if (rmtmp.Count == 0)
                        {
                            _outputManager.AddLogEntry($"No rooms found by the name of {name}, lets try that again.");
                        }
                        else
                        {
                            List<Item> item = _context.Items.Where(i => i.RoomId == rmtmp[0].Id).ToList();
                            var value = _outputManager.GetUserInput($"The {rmtmp[0].Name} has {item.Count} items, yay or nay say you sire (Y or N) to clear them? ");
                            if (value.ToUpper() == "Y" || value.ToUpper() == "Yay" || value.ToUpper() == "Yes")
                            {
                                _inventoryService.ClearItems(item);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _outputManager.AddLogEntry("Not sure that I got that, so lets try again shall we.");
                            }
                        }
                    }
                }
            }
        }
    }
    private void RemovePlayer()
    {
        while(true)
        {
            var input = _outputManager.GetUserInput($"Enter a player name you wish to remove from the game or exit to return to previous menu?");
            if (input != null)
            {
                if (input.ToLower() == "exit")
                {
                    _outputManager.AddLogEntry("Previous Menu...");
                    return;
                }
                else
                {
                    List<Player> players = (List<Player>)_context.Player.Where(obj => obj.Name == input).ToList();
                    if (players.Count() > 0)
                    {
                        int cnt = 0;
                        foreach (Player p in players)
                        {
                            cnt = cnt + 1;
                            _outputManager.AddLogEntry($"{cnt}. {p.Name}");
                        }
                        var value = _outputManager.GetUserInput($"Enter a number or 0 to return to previous? ");
                        int choice = 0;
                        if (Int32.TryParse(value, out choice) == true)
                        {
                            switch (choice)
                            {
                                case 0:
                                    _outputManager.AddLogEntry("Returning to previous menu...");
                                    return;
                                case > 0:
                                    Player tmppc = players[cnt - 1];
                                    bool retval = _playerService.RemovePlayer(tmppc, _context.Rooms, _context.Items, _context.Equipments, _context.Player);
                                    if (retval)
                                    {
                                        _context.SaveChanges();
                                        _outputManager.AddLogEntry("Player removed.");
                                        _outputManager.AddLogEntry("Exit remove player.");
                                    }
                                    else
                                    {
                                        _outputManager.AddLogEntry("Received and error Player was not removed.");
                                        _outputManager.AddLogEntry("Exit remove player.");
                                    }
                                    return;
                                default:
                                    _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                                    continue;
                            }
                        }
                        else
                        {
                            _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                        }
                    }
                    else
                    {
                        _outputManager.AddLogEntry($"My liege I was not able to locate any one named {input}");
                        _outputManager.GetUserInput("Lets try that again shall we, any key to continue?");
                    }
                }
            }
            else
            {
                _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
            }
        }
        return;
    }
    private void UpdatePlayerExperience()
    {
        while (true)
        {
            var input = _outputManager.GetUserInput("Which of our mighty heros has proven worthy of advancement, sire or exit to return to previous?");
            if (input != null)
            {
                if (input.ToLower() == "exit")
                {
                    _outputManager.AddLogEntry("Exit Update Player experience.");
                    return;
                }
                else
                {
                    string name = (string)input;
                    var players = _context.Player.Where(p => p.Name == name).ToList();
                    if (players != null)
                    {
                        if (players.Count == 1)
                        {
                            Player tmppc = players[0];
                            var value = _outputManager.GetUserInput($"Tell me my leige, how much power do the gods wish to bestow on to {tmppc.Name}, exit to return to previous.");
                            if (value.ToLower() == "exit")
                            {
                                _outputManager.AddLogEntry("Exit Update Experience.");
                                return;
                            }
                            else
                            {
                                int choice = 0;
                                if (Int32.TryParse(value, out choice) == true)
                                {
                                    int exppt = choice;
                                    bool retval = _playerService.UpdatePlayerExperience(tmppc, exppt);
                                    if (retval)
                                    {
                                        _context.SaveChanges();
                                        _outputManager.AddLogEntry("Player exeperince updated.");
                                    }
                                    else
                                    {
                                        _outputManager.AddLogEntry("Received and error Player was not updated.");
                                    }
                                    _outputManager.AddLogEntry("Exit Update Player experience.");
                                }
                            }
                        }
                        else
                        {
                            int cnt = 0;
                            _outputManager.AddLogEntry("Heros going by that sir name are as follow your greatness...");
                            foreach (Player pc in players)
                            {
                                cnt = cnt + 1;
                                _outputManager.AddLogEntry($"{cnt}. {pc.Name}");
                            }
                            if (cnt == 0)
                            {
                                _outputManager.AddLogEntry("Absolutely, positutely none, my leige.");
                            }
                            else
                            {
                                var value = _outputManager.GetUserInput($"Enter a number or 0 to return to previous? ");
                                int choice = 0;
                                if (Int32.TryParse(value, out choice) == true)
                                {
                                    switch (choice)
                                    {
                                        case 0:
                                            _outputManager.AddLogEntry("Returning to previous menu...");
                                            return;
                                        case > 0:
                                            Player tmppc = players[cnt - 1];
                                            value = _outputManager.GetUserInput($"Tell me my leige how much power are the gods going to bestow on to {tmppc.Name}, exit to return to previous.");
                                            if (value.ToLower() == "exit")
                                            {
                                                _outputManager.AddLogEntry("Exit Update Experience.");
                                                return;
                                            }
                                            else
                                            {
                                                choice = 0;
                                                if (Int32.TryParse(value, out choice) == true)
                                                {
                                                    int exppt = choice;
                                                    bool retval = _playerService.UpdatePlayerExperience(tmppc, exppt);
                                                    if (retval)
                                                    {
                                                        _context.SaveChanges();
                                                        _outputManager.AddLogEntry("Player exeperince updated.");
                                                    }
                                                    else
                                                    {
                                                        _outputManager.AddLogEntry("Received and error Player was not updated.");
                                                    }
                                                    _outputManager.AddLogEntry("Exit Update Player experience.");
                                                }
                                            }
                                            break;
                                        default:
                                            _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                                            continue;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                    }
                }
            }
            else
            {
                _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
            }
        }
        return;
    }
    private void EquipPlayer()
    {
        while (true)
        {
            var name = _outputManager.GetUserInput("Enter name of player you want to equip or exit to return to previous? ");
            if (name.ToLower() == "exit")
            {
                return;
            }
            else
            {
                if (name != null)
                {
                    var players = _context.Player.Where(p => p.Name == name && p.EquipmentId == null).ToList();
                    if (players.Count == 0)
                    {
                        var input = _outputManager.GetUserInput($"I was not able to find a hero named {name}, perhaps we try again or exit to return to previous? ");
                        if (input.ToLower() == "exit")
                        {
                            _outputManager.AddLogEntry("Returning to previous menu...");
                            return;
                        }
                        else
                        {
                            _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                            continue;
                        }
                    }
                    else
                    {
                        int cnt = 0;
                        foreach (Player p in players)
                        {
                            cnt = cnt + 1;
                            _outputManager.AddLogEntry($"{cnt}. {p.Name}");
                        }
                        var value = _outputManager.GetUserInput($"Enter a number or 0 to return to previous? ");
                        int choice = 0;
                        if (Int32.TryParse(value, out choice) == true)
                        {
                            switch (choice)
                            {
                                case 0:
                                    _outputManager.AddLogEntry("Returning to previous menu...");
                                    return;
                                case > 0:
                                    Player tmppc = players[cnt - 1];
                                    bool retval = AssignEquip(tmppc);
                                    if (retval == true)
                                    {
                                       _outputManager.AddLogEntry("Player equiped.");
                                    }
                                    else
                                    {
                                       _outputManager.AddLogEntry("Received and error Player was not euipped.");
                                    }
                                    continue;
                                default:
                                    _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                                    continue;
                            }
                        }
                        else
                        {
                            _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                        }
                    }
                }
                else
                {
                    _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                }
            }
            _outputManager.AddLogEntry("Exit Equip Player.");
        }
    }

    private bool AssignEquip(Player pc)
    {
        Item tmpitem = new Item();
        Equipment tmpequip = new Equipment();

        while (true)
        {
            _outputManager.AddLogEntry("Select one item from the list below...");
            var input = _outputManager.GetUserInput("Enter 1=Sword,2=Bow & Arrows, 3=Staff, 4=Axe or 0 to exit.");
            int choice = 0;
            if (Int32.TryParse(input, out choice) == true)
            {
                // For speed I just gave them all the same values
                tmpitem.PlayerId = pc.Id;
                tmpitem.Enchanted = false;
                tmpitem.Value = 10;
                tmpitem.Weight = 2;
                tmpitem.RoomId = null;
                tmpitem.Type = "Weapon";
                switch (choice)
                {
                    case 0:
                        return false;
                    case 1:
                        tmpitem.Name = "Sword";
                        tmpitem.Attack = 5;
                        tmpitem.Defense = 1;
                        break;
                    case 2:
                        tmpitem.Name = "Bow & Arrows";
                        tmpitem.Attack = 5;
                        tmpitem.Defense = 0;
                        break;
                    case 3:
                        tmpitem.Name = "Staff";
                        tmpitem.Attack = 1;
                        tmpitem.Defense = 1;
                        break;
                    case 4:
                        tmpitem.Name = "Axe";
                        tmpitem.Attack = 5;
                        tmpitem.Defense = 1;
                        break;
                    default:
                        _outputManager.GetUserInput("Not sure that I got that, so lets try again shall we, any key to continue?");
                        continue;
                }
                _context.Items.Add(tmpitem);
                _context.SaveChanges();
                Item tmpi = _context.Items.Where(i => i.PlayerId == pc.Id).FirstOrDefault();
                tmpequip.WeaponId = tmpi.Id;
                tmpequip.ArmorId = null;
                tmpequip.AccessoryId = null;
                tmpequip.PotionId = null;
                _context.Equipments.Add(tmpequip);
                _context.SaveChanges();
                Equipment tmpe = _context.Equipments.Where(e => e.WeaponId == tmpi.Id).FirstOrDefault();
                pc.EquipmentId = tmpe.Id;
                _context.SaveChanges();
            }
            return true;
        }
    }
    private void PickItUp(List<Item> items)
    {
        //TODO Implement PickItUp
        while (true)
        {
            _outputManager.AddLogEntry("Which items do you want to add to your magical 'Bag of Holding'?");
            foreach (Item item in items)
            {
                var input = _outputManager.GetUserInput($"Pick up {item.Name}, yay or nay say you, my leige or exit to return to previous? (Y or N)");
                if (input != null)
                {
                    if (input.ToUpper() == "Y" || input.ToUpper() == "Yay" || input.ToUpper() == "Yes")
                    {
                        _inventoryService.PickUpItem(item, _player);
                        _context.SaveChanges();
                    }
                    else
                    {
                        if (input == "exit")
                        {
                            return;
                        }
                        else
                        {
                            _outputManager.AddLogEntry("Not sure that I got that, so lets try again shall we.");
                            continue;
                        }
                    }
                }
                else
                {
                    _outputManager.AddLogEntry("Seems a shame to leave all this behind but your wish is my command sire.");
                    return;
                }
            }
        }
    }
    private void PlayerActionMenu2()
    {
        //TODO Implement PlayerActionMenu2
        AttackCharacter();
        return;
    }
    private void ListRoomContents()
    {
        //TODO Implement ListRoomContents
        _outputManager.AddLogEntry("Not yet implemented...");
        return;
    }
    private void FindItems()
    {
        //TODO Implement FindItems
        _outputManager.AddLogEntry("Not yet implemented...");
        return;
    }
    private void InventoryMenu()
    {
        //TODO Implement InventoryMenu
        _outputManager.AddLogEntry("Not yet implemented...");
        return;
    }
    private void FindOccupiedRooms()
    {
        //TODO Implement FindOccupiedRooms

        _outputManager.AddLogEntry("Not yet implemented...");
        return;
    }
}