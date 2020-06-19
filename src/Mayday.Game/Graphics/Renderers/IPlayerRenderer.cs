using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Graphics.Renderers
{
    public interface IPlayerRenderer
    {

        void DrawPlayers(IEnumerable<IEntity> players);
        void DrawPlayer(IEntity player);
        
    }
}