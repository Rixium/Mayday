namespace Mayday.Game.ECS.Components
{
    public interface IComponent
    {
        IEntity Entity { get; set; }
        void Update();
        void Draw();
    }
}