using System;
using System.Drawing;
using System.Linq;
using Mayday.Game.Gameplay.Items;
using Mayday.UI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Yetiface.Engine;
using Color = System.Drawing.Color;

namespace Mayday.Game.UI.Controllers
{
    public class GameScreenUserInterfaceController
    {
        private int _currentSelection = 0;
        public GameScreenUserInterface UserInterface { get; set; }
        public Action<int> SelectedItemSlotChanged { get; set; }

        public GameScreenUserInterfaceController(GameScreenUserInterface userInterface)
        {
            UserInterface = userInterface;
            
            var renderable = new TextureRegion(YetiGame.ContentManager.Load<Texture2D>("UI/bar_inventory_item"));
            
            foreach (var background in UserInterface.InventorySlotBackgrounds)
            {
                background.Renderable = renderable;
            }
            
            foreach (var background in UserInterface.MainInventorySlotBackgrounds)
            {
                background.Renderable = renderable;
            }
            
        }

        public void SetBarSlotData(Texture2D itemImage, int count, int index)
        {
            UserInterface.InventorySlotItemImages[index].Renderable = new TextureRegion(itemImage);
            UserInterface.InventorySlotItemCounts[index].Text = $"{count}";
        }
        
        public void SetInventorySlotData(Texture2D itemImage, int count, int index)
        {
            UserInterface.MainInventorySlotItemImages[index].Renderable = new TextureRegion(itemImage);
            UserInterface.MainInventorySlotItemCounts[index].Text = $"{count}";
        }

        public void InventoryBarChanged(IInventory inventory)
        {
            var stackIndex = inventory.Slots - 1;
            
            foreach (var stack in inventory.ItemStacks.Reverse())
            {
                stackIndex--;
                
                if (stack.IsEmpty())
                {
                    continue;
                }
                
                SetBarSlotData(ContentChest.ItemTextures[stack.Item.Id], stack.Count, stackIndex + 1);
            }
        }

        public void MainInventoryChanged(IInventory inventory)
        {
            var stackIndex = inventory.Slots - 1;
            
            foreach (var stack in inventory.ItemStacks.Reverse())
            {
                stackIndex--;
                
                if (stack.IsEmpty())
                {
                    continue;
                }
                
                SetInventorySlotData(ContentChest.ItemTextures[stack.Item.Id], stack.Count, stackIndex + 1);
            }
        }

        public void ToggleMainInventory()
        {
            UserInterface.InventoryPanel.Visible = !UserInterface.InventoryPanel.Visible;
        }

        public void InventorySelectionChanged(int selection)
        {
            UserInterface.InventorySlotBackgrounds[_currentSelection].Color = Microsoft.Xna.Framework.Color.White;
            if (_currentSelection == selection) return;
            _currentSelection = selection;
            UserInterface.InventorySlotBackgrounds[selection].Color = Microsoft.Xna.Framework.Color.Green * 0.5f;
            SelectedItemSlotChanged?.Invoke(_currentSelection);
        }
        
            
    }
}