using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Components
{
    public class JumpComponent : IComponent
    {
        public IPlayer Player { get; set; }
        private bool _jumping;
        
        public void Update()
        {
            
        }

        public void OnAddedToPlayer()
        {
            // We need to know when the player hits the floor, so that we can
            // set jumping back to false.
            var gravityComponent = Player.GetComponent<GravityComponent>();
            gravityComponent.HitFloor += () => _jumping = false;
        }

        public void Jump()
        {
            if (_jumping) return;
            
            var moveComponent = Player.GetComponent<MoveComponent>();
            
            _jumping = true;
            moveComponent.YVelocity = 4;
        }

    }
}