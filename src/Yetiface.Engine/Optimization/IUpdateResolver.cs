namespace Yetiface.Engine.Optimization
{
    public interface IUpdateResolver<in T>
    {
        bool ShouldUpdate(T obj);
    }
}