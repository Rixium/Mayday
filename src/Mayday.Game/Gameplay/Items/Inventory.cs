using System;
using System.Collections.Generic;

namespace Mayday.Game.Gameplay.Items
{
    public class Inventory : IInventory
    {
        public Action<IItem> ItemPickup { get; set; }
        public Action InventoryChanged { get; set; }
        public IList<IItemStack> ItemStacks { get; set; }
        public int Slots { get; set; }

        public Inventory(int slotCount)
        {
            Slots = slotCount;
            InitializeSlots();
            
            ItemPickup += (item) => InventoryChanged?.Invoke();
        }
        
        private void InitializeSlots()
        {
            ItemStacks = new List<IItemStack>();
            
            for(var i = 0; i < Slots; i++)
                ItemStacks.Add(new ItemStack());
        }
        
        public bool AddItemToInventory(IItem item)
        {
            var selectedStack = GetStackForItem(item);
            var successful = selectedStack?.AddItem(item);

            if (!successful.HasValue || !successful.Value) 
                return false;
            
            ItemPickup?.Invoke(item);
            return true;
        }
        
        private IItemStack GetStackForItem(IItem item)
        {
            IItemStack selectedStack = null;

            if (ItemStacks == null)
                ItemStacks = new List<IItemStack>();

            foreach (var stack in ItemStacks)
            {
                if (stack.IsEmpty())
                    selectedStack = stack;
                if (!stack.ContainsItemOfType(item))
                    continue;
                if (!stack.HasSpaceFor(item))
                    continue;
                selectedStack = stack;
                break;
            }

            return selectedStack ?? CreateNewStackIfPossible();
        }

        private IItemStack CreateNewStackIfPossible()
        {
            if (ItemStacks == null)
                ItemStacks = new List<IItemStack>();

            if (ItemStacks.Count >= Slots)
                return null;

            var newStack = new ItemStack();
            ItemStacks.Add(newStack);
            return newStack;
        }

    }
}