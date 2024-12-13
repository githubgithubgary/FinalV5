namespace ConsoleRpg.Helpers;

public class MenuManager
{
    private readonly OutputManager _outputManager;

    public MenuManager(OutputManager outputManager)
    {
        _outputManager = outputManager;
    }
    public int ShowMainMenu()
    {
        _outputManager.AddLogEntry("Welcome to the RPG Game!");
        _outputManager.AddLogEntry("1. Start Game");
        _outputManager.AddLogEntry("0. Exit");

        return HandleMainMenuInput();
    }

    private int HandleMainMenuInput()
    {
        while (true)
        {
            var input = _outputManager.GetUserInput("Selection:");
            switch (input)
            {
                case "1":
                    _outputManager.AddLogEntry("Starting game...");
                    return 1;
                case "0":
                    _outputManager.AddLogEntry("Exiting game...");
                    Environment.Exit(0);
                    return 0;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    break;
            }
        }
    }
    public int ShowPlayerMaintenanceMenu()
    {
        _outputManager.AddLogEntry("Player Maintenance Menu");
        _outputManager.AddLogEntry("1. List Active Player");
        _outputManager.AddLogEntry("2. Add a Player");
        _outputManager.AddLogEntry("3. Edit a Player");
        _outputManager.AddLogEntry("4. Equip a Player");
        _outputManager.AddLogEntry("5. List all Players");
        _outputManager.AddLogEntry("0. Previous Menu");

        return HandlePlayerMaintenanceMenuInput();
    }

    private int HandlePlayerMaintenanceMenuInput()
    {
        while (true)
        {
            var input = _outputManager.GetUserInput("Selection:");
            switch (input)
            {
                case "1": // "List active Player..."
                    return 1;
                case "2": // "Add a Player..."
                    return 2;
                case "3": // "Edit Player..."
                    return 3;
                case "4": // "Equip Players..."
                    return 4;
                case "5": // "List Players..."
                    return 5;
                case "0": // "Previous Menu..."
                    _outputManager.AddLogEntry("Previous Menu...");
                    return 0;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    continue;
            }
        }
    }
    public int ShowEditActivePlayerMenu()
    {
        _outputManager.AddLogEntry("Edit Active Player Menu");
        _outputManager.AddLogEntry("1. Activate other Player");
        _outputManager.AddLogEntry("2. Edit Health");
        _outputManager.AddLogEntry("3. Edit Armor");
        _outputManager.AddLogEntry("4. Edit Weapon");
        _outputManager.AddLogEntry("5. Edit Potion");
        _outputManager.AddLogEntry("6. Edit Accessory");
        _outputManager.AddLogEntry("0. Previous Menu");

        return HandleEditActivePlayerMenuInput();
    }
    private int HandleEditActivePlayerMenuInput()
    {
        while (true)
        {
            var input = _outputManager.GetUserInput("Selection:");
            switch (input)
            {
                case "1": // "Activate another Player..."
                    return 1;
                case "2": // "Edit Player Health..."
                    return 2;
                case "3": // "Edit Player Armor..."
                    return 3;
                case "4": // "Edit Player Weapon..."
                    return 4;
                case "5": // "Edit Player Potion..."
                    return 5;
                case "6": // "Edit Player Accessory..."
                    return 6;
                case "0": // "Previous Menu..."
                    _outputManager.AddLogEntry("Previous Menu...");
                    return 0;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    continue;
            }
        }
    }
    public int ShowPlayerActionMenu()
    {
        while (true)
        {
            _outputManager.AddLogEntry("Player Action Menu");
            _outputManager.AddLogEntry("1. Explore dungeon");
            _outputManager.AddLogEntry("0. Previous Menu");
            var input = _outputManager.GetUserInput("Selection:");
            switch (input)
            {
                case "1": // "Activate another Player..."
                    return 1;
//                case "2": // "Edit Player Health..."
//                    return 2;
//                case "3": // "Edit Player Armor..."
//                    return 3;
//                case "4": // "Edit Player Weapon..."
//                    return 4;
//                case "5": // "Edit Player Potion..."
//                    return 5;
//                case "6": // "Edit Player Accessory..."
//                    return 6;
                case "0": // "Previous Menu..."
                    _outputManager.AddLogEntry("Previous Menu...");
                    return 0;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    continue;
            }
        }
    }

    public int ShowAdminMenu()
    {
        while (true)
        {
            _outputManager.AddLogEntry("Administration Menu");
            _outputManager.AddLogEntry("1. Remove a Player");
            _outputManager.AddLogEntry("2. Update Player Experience");
            _outputManager.AddLogEntry("3. Find Item");
            _outputManager.AddLogEntry("4. Find Occupied Rooms");
            _outputManager.AddLogEntry("5. Clear rooms of all items");
            _outputManager.AddLogEntry("6. List room contents");
            _outputManager.AddLogEntry("0. Previous Menu");

            var input = _outputManager.GetUserInput("Selection:");
       
            switch (input)
            {
                case "1":
                    return 1;
                case "2":
                    return 2;
                case "3":
                    return 3;
                case "4":
                    return 4;
                case "5":
                    return 5;
                case "6":
                    return 6;
                case "0":
                    return 0;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose again.");
                    continue;
            }
        }
    }
}
