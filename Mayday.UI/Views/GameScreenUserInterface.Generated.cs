/* Generated by MyraPad at 05/06/2020 20:20:06 */

using System.Collections.Generic;
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

			var horizontalStackPanel1 = new HorizontalStackPanel();
			horizontalStackPanel1.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;
			horizontalStackPanel1.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Top;

			for (var i = 0; i < 8; i++)
			{
				var panel = new Panel();
				panel.Width = 64;
				panel.Height = 64;
				
				var inventorySlotBackground = new Image();
				inventorySlotBackground.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
				inventorySlotBackground.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Stretch;

				var inventorySlotItemImage = new Image();
				inventorySlotItemImage.Width = 40;
				inventorySlotItemImage.Height = 40;
				inventorySlotItemImage.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;
				inventorySlotItemImage.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Center;

				var inventorySlotCount = new Label();
				inventorySlotCount.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
				inventorySlotCount.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Bottom;

				InventorySlotBackgrounds.Add(inventorySlotBackground);
				InventorySlotItemImages.Add(inventorySlotItemImage);
				InventorySlotItemCounts.Add(inventorySlotCount);
				
				panel.Widgets.Add(inventorySlotBackground);
				panel.Widgets.Add(inventorySlotItemImage);
				panel.Widgets.Add(inventorySlotCount);
				
				horizontalStackPanel1.Widgets.Add(panel);
			}

			Padding = new Thickness(10);
			Widgets.Add(ItemListBox);
			Widgets.Add(horizontalStackPanel1);
		}
		
		public ListBox ItemListBox;
		
		public IList<Image> InventorySlotItemImages = new List<Image>();
		public IList<Label> InventorySlotItemCounts = new List<Label>();
		public IList<Image> InventorySlotBackgrounds = new List<Image>();
		
	}
}