using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Components
{
    public class JumpComponent : IComponent
    {
        public IPlayer Player { get; set; }

        public bool Jumping { get; set; }
        
        public void Update()
        {
            
        }

        public void OnAddedToPlayer()
        {
            // We need to know when the player hits the floor, so that we can
            // set jumping back to false.
            var gravityComponent = Player.GetComponent<GravityComponent>();
            gravityComponent.HitFloor += () => Jumping = false;
        }

        public void Jump()
        {
            if (Jumping) return;
            
            var moveComponent = Player.GetComponent<MoveComponent>();
            
            Jumping = true;
            moveComponent.YVelocity = 4;
        }

    }
}