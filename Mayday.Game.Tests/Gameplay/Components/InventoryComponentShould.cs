using System;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Items;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Mayday.Game.Tests.Gameplay.Components
{
    public class InventoryComponentShould
    {

        [Test]
        public void CreateNewStackWhenNoneAvailable()
        {
            var inventoryComponent = new InventoryComponent
            {
                Slots = 1
            };
            
            inventoryComponent.AddItemToInventory(Substitute.For<IItem>());
            
            inventoryComponent.Slots.ShouldBe(1);
        }
        
        
        [Test]
        public void NotCreateNewStackWhenSlotCountReached()
        {
            var inventoryComponent = new InventoryComponent
            {
                Slots = 1
            };
            
            inventoryComponent.AddItemToInventory(Substitute.For<IItem>());
            inventoryComponent.AddItemToInventory(Substitute.For<IItem>());
            
            inventoryComponent.Slots.ShouldBe(1);
        }
        
        [Test]
        public void CreateNewStackForSameItemIfOtherStackMaxCapacity()
        {
            var inventoryComponent = new InventoryComponent
            {
                Slots = 2
            };
            
            const string itemName = "TestItem";
            var itemOne = Substitute.For<IItem>();
            itemOne.Name = itemName;
            itemOne.MaxStackSize = 1;
            var itemTwo = Substitute.For<IItem>();
            itemTwo.Name = itemName;
            itemTwo.MaxStackSize = 1;

            itemOne.IsTheSameAs(itemTwo).Returns(true);
            
            inventoryComponent.AddItemToInventory(itemOne);
            inventoryComponent.AddItemToInventory(itemTwo);
            
            inventoryComponent.Slots.ShouldBe(2);
            inventoryComponent.ItemStacks[0].Count.ShouldBe(1);
            inventoryComponent.ItemStacks[1].Count.ShouldBe(1);
            inventoryComponent.ItemStacks[0].Item.Name.ShouldBe(itemName);
            inventoryComponent.ItemStacks[1].Item.Name.ShouldBe(itemName);
        }
        
        
        [Test]
        public void FailToCreateNewStackForSameItemIfMaxSlots()
        {
            var inventoryComponent = new InventoryComponent
            {
                Slots = 1
            };
            
            const string itemName = "TestItem";
            var itemOne = Substitute.For<IItem>();
            itemOne.Name = itemName;
            itemOne.MaxStackSize = 1;
            var itemTwo = Substitute.For<IItem>();
            itemTwo.Name = itemName;
            itemTwo.MaxStackSize = 1;

            itemOne.IsTheSameAs(itemTwo).Returns(true);
            
            inventoryComponent.AddItemToInventory(itemOne);
            inventoryComponent.AddItemToInventory(itemTwo);
            
            inventoryComponent.Slots.ShouldBe(1);
            inventoryComponent.ItemStacks[0].Count.ShouldBe(1);
            inventoryComponent.ItemStacks[0].Item.Name.ShouldBe(itemName);
            Should.Throw<ArgumentOutOfRangeException>(() => inventoryComponent.ItemStacks[1]);
        }

    }
}