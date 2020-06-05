using Mayday.UI.Views;
using Microsoft.Xna.Framework;
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
        
        public void AddItem(string item)
        {
            UserInterface.ItemListBox.Items.Insert(0, new ListItem(item, Color.White));
            UserInterface.ItemListBox.InvalidateMeasure();
        }

        public void ClearItems()
        {
            UserInterface.ItemListBox.Items.Clear();
            UserInterface.ItemListBox.InvalidateMeasure();
        }

    }
}