using System;
using Mayday.Game.Gameplay.Items;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Mayday.Game.Tests.Gameplay.Items
{
    public class InventoryComponentShould
    {

        [Test]
        public void CreateNewStackWhenNoneAvailable()
        {
            var inventory = new Inventory(1);
            
            inventory.AddItemToInventory(Substitute.For<IItem>());
            
            inventory.Slots.ShouldBe(1);
        }
        
        
        [Test]
        public void NotCreateNewStackWhenSlotCountReached()
        {
            var inventory = new Inventory(1);

            inventory.AddItemToInventory(Substitute.For<IItem>());
            inventory.AddItemToInventory(Substitute.For<IItem>());
            
            inventory.Slots.ShouldBe(1);
        }
        
        [Test]
        public void CreateNewStackForSameItemIfOtherStackMaxCapacity()
        {
            var inventory = new Inventory(2);
            
            const string itemName = "TestItem";
            var itemOne = Substitute.For<IItem>();
            itemOne.Name = itemName;
            itemOne.MaxStackSize = 1;
            var itemTwo = Substitute.For<IItem>();
            itemTwo.Name = itemName;
            itemTwo.MaxStackSize = 1;

            itemOne.IsTheSameAs(itemTwo).Returns(true);
            
            inventory.AddItemToInventory(itemOne);
            inventory.AddItemToInventory(itemTwo);
            
            inventory.Slots.ShouldBe(2);
            inventory.ItemStacks[0].Count.ShouldBe(1);
            inventory.ItemStacks[1].Count.ShouldBe(1);
            inventory.ItemStacks[0].Item.Name.ShouldBe(itemName);
            inventory.ItemStacks[1].Item.Name.ShouldBe(itemName);
        }
        
        [Test]
        public void FailToCreateNewStackForSameItemIfMaxSlots()
        {
            var inventory = new Inventory(1);
            
            const string itemName = "TestItem";
            var itemOne = Substitute.For<IItem>();
            itemOne.Name = itemName;
            itemOne.MaxStackSize = 1;
            var itemTwo = Substitute.For<IItem>();
            itemTwo.Name = itemName;
            itemTwo.MaxStackSize = 1;

            itemOne.IsTheSameAs(itemTwo).Returns(true);
            
            inventory.AddItemToInventory(itemOne);
            inventory.AddItemToInventory(itemTwo);
            
            inventory.Slots.ShouldBe(1);
            inventory.ItemStacks[0].Count.ShouldBe(1);
            inventory.ItemStacks[0].Item.Name.ShouldBe(itemName);
            Should.Throw<ArgumentOutOfRangeException>(() => inventory.ItemStacks[1]);
        }

    }
}