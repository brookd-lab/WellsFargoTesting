using InventoryManagement;
using System;

namespace InventoryManagement
{
    public class InventoryService
    {
        private Inventory Inventory { get; }

        public InventoryService(Inventory inventory)
        {
            if (inventory is null)
                throw new ArgumentNullException(nameof(inventory));
            Inventory = inventory;
        }

     
    }
}

