using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Game.Gameplay.Components
{
    public class InventoryComponent : IComponent
    {
        public IPlayer Player { get; set; }
        
        public Action<IItem> ItemPickup { get; set; }
        public Action InventoryChanged { get; set; }
        public IList<IItemStack> ItemStacks { get; set; }
        public int Slots { get; set; }

        public InventoryComponent(int slots)
        {
            Slots = slots;
            InitializeSlots();
            
            ItemPickup += (item) => InventoryChanged?.Invoke();
        }

        private void InitializeSlots()
        {
            ItemStacks = new List<IItemStack>();
            
            for(var i = 0; i < Slots; i++)
                ItemStacks.Add(new ItemStack());
        }

        public void AddItemToInventory(IItem item)
        {
            var selectedStack = GetStackForItem(item);
            var successful = selectedStack?.AddItem(item);

            if (successful.HasValue && successful.Value)
            {
                ItemPickup?.Invoke(item);
            }
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

        public void Update()
        {
        }

        public void OnAddedToPlayer()
        {
        }
    }
}