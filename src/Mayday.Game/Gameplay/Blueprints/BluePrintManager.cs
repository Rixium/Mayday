using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking;
using Mayday.Game.Screens;

namespace Mayday.Game.Gameplay.Blueprints
{
    public class BluePrintManager : IBluePrintManager
    {
        private readonly GameScreen _gameScreen;

        public BluePrintManager(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        public void SetupFor(IEntity entity)
        {
            if (entity is Tile tile) 
                SetupTileBlueprint(tile);
        }

        private void SetupTileBlueprint(Tile tile)
        {
            // TODO OPTIMISE
            // var itemDropType = tile.TileProperties?.ItemDropType;
            //
            // if (itemDropType == null || itemDropType == ItemTypes.None) return;
            //
            // var itemDropperComponent = tile.AddComponent(
            //     new ItemDropperComponent(itemDropType));
            //
            // itemDropperComponent.ItemDrop += (entity, itemDrop) =>
            // {
            //     entity.GameArea.DropItem(itemDrop);
            //     PacketManager.SendItemDropPacket(itemDrop);
            // };
        }
    }
}