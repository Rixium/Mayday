using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Entities
{
    public interface IPlayer
    {
        ulong SteamId { get; set; }
        int X { get; set; }
        int Y { get; set; }

        int HeadId { get; set; }
        int BodyId { get; set; }
        int LegsId { get; set; }
        
        int XDirection { get; set; }
        int FacingDirection { get; set; }
        IGameWorld GameWorld { get; set; }
        Vector2 Position { get; }
        Vector2 Center { get; }

        void Update();

        IComponent AddComponent(IComponent component);
        T GetComponent<T>() where T : IComponent;
        
        Rectangle GetBounds();
        
    }
}