using System.Collections.Generic;
using Mayday.UI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace Mayday.UI.Controllers
{
    public class GameScreenUserInterfaceController
    {
        public GameScreenUserInterface UserInterface { get; set; }

        public GameScreenUserInterfaceController(GameScreenUserInterface userInterface)
        {
            UserInterface = userInterface;
        }

        public void SetSlotData(Texture2D itemImage, int count, int index)
        {
            UserInterface.InventorySlotItemImages[index].Renderable = new TextureRegion(itemImage);
            UserInterface.InventorySlotItemCounts[index].Text = $"{count}";
        }

    }
}