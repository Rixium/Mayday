namespace Yetiface.Engine.ECS.Components
{
    public abstract class Component : IComponent
    {
        public IEntity Entity { get; set; }

        public abstract void Update();

        public abstract void Draw();
    }
}