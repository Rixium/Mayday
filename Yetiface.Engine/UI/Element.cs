using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI
{
    public class Element : IElement
    {
        private bool _debug;
        public IUserInterface UserInterface { get; set; }

        public IElement Parent { get; set; }
        public IList<IElement> Children { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public IElement AddElement(IElement element)
        {
            if (Children == null) Children = new List<IElement>();
            element.Parent = this;
            Children.Add(element);
            
            return element;
        }

        public void Update()
        {
            if (Children == null) return;

            foreach (var element in Children) element.Update();
        }

        public virtual void Draw()
        {
            if (Children == null) return;

            foreach (var element in Children) element.Draw();
        }

        public virtual void DrawDebug()
        {
            GraphicsUtils.Instance.DrawRectangle(X, Y, Width, Height, Color.Red);

            if (Children == null) return;

            foreach (var element in Children) element.DrawDebug();
        }
    }
}