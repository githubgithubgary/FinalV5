using ConsoleRpgEntities.Models.Equipments;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System;
using ConsoleRpgEntities.Models.Rooms;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Services
{
    public class InventoryService
    {
        private readonly IOutputService _outputService;

        public InventoryService(IOutputService outputService)
        {
            _outputService = outputService;
        }

        public void ListItems(List<Item> Items)
        {
            _outputService.WriteLine("Inventory:");
            foreach (var item in Items)
            {
                _outputService.WriteLine($"{item.Name}");
            }
        }
        public void ClearItems(List<Item> Items)
        {
            foreach (var item in Items)
            {
                item.RoomId = null;
            }
        }
        public void PickUpItem(Item item, Player pc)
        {
            item.PlayerId = pc.Id;
            item.RoomId = null;
        }

    }
}
