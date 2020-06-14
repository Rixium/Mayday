using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Packets;
using Steamworks;
using Yetiface.Engine.Networking;

namespace Mayday.Game.Networking
{
    public class PacketManager
    {

        public static void Initialize(INetworkManager networkManager)
        {
            NetworkManager = networkManager;
        }

        public static INetworkManager NetworkManager { get; set; }

        public static void SendJumpPacket(JumpComponent jumpComponent)
        {
            var jumpPacket = new JumpPacket
            {
                SteamId = jumpComponent.Entity.EntityId
            };

            var package = NetworkManager.MessagePackager.Package(jumpPacket);
            NetworkManager.SendMessage(package);
        }
        
        public static void SendTileChangePacket(Tile tile)
        {
            var tileChangePacket = new TileTypePacket()
            {
                X = tile.TileX,
                Y = tile.TileY,
                TileType = (int) tile.TileType
            };

            var package = NetworkManager.MessagePackager.Package(tileChangePacket);
            
            NetworkManager.SendMessage(package);
        }
        
        public static void SendItemDropPacket(ItemDrop itemDrop)
        {
            var itemDropPacket = new ItemDropPacket()
            {
                X = itemDrop.X,
                Y = itemDrop.Y,
                ItemId = (int) itemDrop.Item.ItemId
            };

            var package = NetworkManager.MessagePackager.Package(itemDropPacket);
            
            NetworkManager.SendMessage(package);
        }
        
        public static void SendJumpStatePacket(JumpPacket jumpPacket)
        {
            jumpPacket.SteamId = SteamClient.SteamId;
            var package = NetworkManager.MessagePackager.Package(jumpPacket);
            NetworkManager.SendMessage(package);
        }
        
        public static void SendPositionPacket(IComponent moveComponent)
        {
            var entity = moveComponent.Entity;
            
            var position = new PlayerPositionPacket
            {
                X = (int) entity.X,
                Y = (int) entity.Y,
                SteamId = moveComponent.Entity.EntityId
            };

            var package = NetworkManager.MessagePackager.Package(position);
                
            NetworkManager.SendMessage(package);
        }

        public static void SendMoveDirectionPacket(IComponent moveComponent)
        {
            var entity = moveComponent.Entity;
            
            var data = new PlayerMovePacket()
            {
                XDirection = entity.XDirection,
                SteamId = moveComponent.Entity.EntityId
            };
            
            var movePackage = NetworkManager.MessagePackager.Package(data);
            NetworkManager.SendMessage(movePackage);
        }
        
    }
}