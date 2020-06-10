using Mayday.Game.Networking.Packets;
using Yetiface.Engine.Networking.Packagers;

namespace Mayday.Game.Networking.Packagers
{
    public class MaydayMessagePackager : NetworkMessagePackager
    {

        public MaydayMessagePackager()
        {
            AddDefinition<TileTypePacket>();
            AddDefinition<MapRequestPacket>();
            AddDefinition<PlayerMovePacket>();
            AddDefinition<PlayerPositionPacket>();
            AddDefinition<JumpPacket>();
            AddDefinition<ChatMessagePacket>();
            AddDefinition<NewPlayerPacket>();
            AddDefinition<ItemDropPacket>();
            AddDefinition<MapSendCompletePacket>();
        }
        
    }
}