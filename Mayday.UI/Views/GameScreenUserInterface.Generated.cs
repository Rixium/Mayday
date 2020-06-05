/* Generated by MyraPad at 05/06/2020 18:54:11 */
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
#endif

namespace Mayday.UI.Views
{
	partial class GameScreenUserInterface: Panel
	{
		private void BuildUI()
		{
			ItemListBox = new ListBox();
			ItemListBox.Id = "ItemListBox";

			
			Padding = new Thickness(10);
			Widgets.Add(ItemListBox);
		}

		
		public ListBox ItemListBox;
	}
}