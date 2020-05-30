using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Graphics.Renderers
{
    public interface IPlayerRenderer
    {

        void DrawPlayers(Dictionary<ulong, IPlayer> players);
        void DrawPlayer(IPlayer player);
        
    }
}