using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Components
{
    public class JumpComponent : IComponent
    {

        public IEntity Entity { get; set; }
        public bool Jumping { get; set; }

        public void Update()
        {
            
        }

        public void OnAddedToPlayer()
        {
            // We need to know when the player hits the floor, so that we can
            // set jumping back to false.
            var moveComponent = Entity.GetComponent<MoveComponent>();
            moveComponent.HitFloor += () => Jumping = false;
        }

        public void Jump()
        {
            if (Jumping) return;
            
            var moveComponent = Entity.GetComponent<MoveComponent>();
            
            Jumping = true;
            moveComponent.YVelocity = 2 * Game1.GlobalGameScale;;
        }

    }
}