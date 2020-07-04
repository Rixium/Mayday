using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Game.Gameplay.Collections
{
    public class PlayerComponentSet
    {
        public MoveComponent MoveComponent { get; set; }
        public GravityComponent GravityComponent { get; set; }
        public JumpComponent JumpComponent { get; set; }
        public IInventory MainInventory { get; set; }
        public IInventory BarInventory { get; set; }
        public ItemPickerComponent ItemPickerComponent { get; set; }
        public PlayerAnimationComponent PlayerAnimationComponent { get; set; }
        public ItemPlacerComponent ItemPlacerComponent { get; set; }
        public CharacterControllerComponent CharacterControllerComponent { get; set; }
        public BlockBreakerComponent BlockBreakerComponent { get; set; }
    }
}