namespace Yetiface.Engine.Data
{
    public class AsepriteMetaData
    {
        public string App { get; set; }
        public string Image { get; set; }
        public AsepriteFrameTag[] FrameTags { get; set; }
        public AsepriteSlice[] Slices { get; set; }
    }
}