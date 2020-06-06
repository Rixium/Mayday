using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Graphics.Renderers
{
    public interface IPlayerRenderer
    {

        void DrawPlayers(Dictionary<ulong, Player> players);
        void DrawPlayer(Player player);
        
    }
}