using Mayday.Game.Gameplay.Items;
using Mayday.UI.Views;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Yetiface.Engine;

namespace Mayday.Game.UI.Controllers
{
    public class GameScreenUserInterfaceController
    {
        public GameScreenUserInterface UserInterface { get; set; }

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
            
            foreach (var stack in inventory.ItemStacks)
            {
                stackIndex--;
                
                if (stack.IsEmpty())
                {
                    continue;
                }
                
                SetBarSlotData(ContentChest.Items[stack.Item.Id], stack.Count, stackIndex + 1);
            }
        }

        public void MainInventoryChanged(IInventory inventory)
        {
            var stackIndex = inventory.Slots - 1;
            
            foreach (var stack in inventory.ItemStacks)
            {
                stackIndex--;
                
                if (stack.IsEmpty())
                {
                    continue;
                }
                
                SetInventorySlotData(ContentChest.Items[stack.Item.Id], stack.Count, stackIndex + 1);
            }
        }

        public void ToggleMainInventory()
        {
            UserInterface.InventoryPanel.Visible = !UserInterface.InventoryPanel.Visible;
        } 
            
    }
}