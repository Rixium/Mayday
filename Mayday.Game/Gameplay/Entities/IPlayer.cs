using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Entities
{
    public interface IPlayer
    {
        ulong SteamId { get; set; }
        float X { get; set; }
        float Y { get; set; }

        int HeadId { get; set; }
        int BodyId { get; set; }
        int LegsId { get; set; }
        
        int XDirection { get; set; }
        int FacingDirection { get; set; }
        IGameWorld GameWorld { get; set; }
        Vector2 Position { get; }
        Vector2 Center { get; }

        void Update();

        T AddComponent<T>(T component) where T : IComponent;
        T GetComponent<T>() where T : IComponent;
        
        RectangleF GetBounds();
        
    }
}