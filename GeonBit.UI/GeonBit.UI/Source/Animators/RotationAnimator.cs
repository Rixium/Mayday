namespace GeonBit.UI.Animators
{
    /// <summary>
    /// 
    /// </summary>
    public class RotationAnimator : IAnimator
    {
        
        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            TargetEntity.Rotation += (float) UserInterface.Active.CurrGameTime.ElapsedGameTime.TotalSeconds * 0.05f;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Reset()
        {
            TargetEntity.Rotation = 0;
        }
        
    }
}