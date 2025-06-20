using InventoryManagement;
using System;

namespace InventoryManagement
{
    public class InventoryService
    {
        private IInventory Inventory { get; }

        public InventoryService(IInventory inventory)
        {
            if (inventory is null)
                throw new ArgumentNullException(nameof(inventory));
            Inventory = inventory;
        }

     
    }
}

