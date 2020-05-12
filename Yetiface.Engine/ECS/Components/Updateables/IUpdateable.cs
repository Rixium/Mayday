namespace Yetiface.Engine.ECS.Components.Updateables
{
    
    /// <summary>
    /// Using this allows the screen to pick up that it needs to be updated and contains some kind of
    /// update logic.
    /// </summary>
    public interface IUpdateable : IComponent
    {
        void Update();
    }
}