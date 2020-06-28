using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class GravityComponent : IUpdateable
    {
        private MoveComponent _moveComponent;
        public float Gravity { get; set; } = 7f;

        public IEntity Entity { get; set; }

        public void Update()
        {
            var activeGravity = Gravity;
            
            _moveComponent.YVelocity -= activeGravity * Time.DeltaTime;
            
            _moveComponent.YVelocity = MathHelper.Clamp(_moveComponent.YVelocity, -activeGravity, _moveComponent.YVelocity);
        }

        public void OnAddedToEntity()
        {
            _moveComponent = Entity.GetComponent<MoveComponent>();
        }
    }
}