using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine.Utils;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Yetiface.Engine.UI
{
    public abstract class Element : IElement
    {
        private bool _debug;

        public IUserInterface UserInterface { get; set; }

        public IElement Parent { get; set; }
        public IList<IElement> Children { get; set; }

        /// <summary>
        /// When active, render and allow for user input.
        /// Otherwise, don't render and don't update.
        /// All elements are active by default.
        /// </summary>
        public bool Active { get; set; } = true;

        public Vector2 Offset { get; set; }
        
        public Vector2 Size { get; set; } = new Vector2(1, 1);

        public Vector2 RelativePosition { get; set; }
        
        public Anchor Anchor { get; set; } = Anchor.Center;

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
        
        /// <summary>
        ///  The render rectangle will keep track of the actual render position of the element, and should be updated
        ///  in regards to the elements parent.
        /// </summary>

        public Rectangle RenderRectangle => _renderRectangle;
        
        public bool IsHovering { get; set; }

        private Rectangle _renderRectangle;

        public Color FillColor { get; set; }
        
        public Action<IElement> OnHover { get; set; }
        
        public Action<IElement> OnEnter { get; set; }
        public Action<IElement> OnLeave { get; set; }
        public Action<IElement> OnClicked { get; set; }

        public T AddElement<T>(T element) where T : IElement
        {
            if (Children == null) Children = new List<IElement>();
            
            element.Parent = this;
            element.UserInterface = UserInterface;
            
            Children.Add(element);

            return element;
        }

        public IElement GetElementUnderMouse(Rectangle mouseBounds)
        {
            if (!mouseBounds.Intersects(_renderRectangle)) return null;
            
            foreach (var child in Children)
            {
                var childHit = child.GetElementUnderMouse(mouseBounds);
                if (childHit != null) 
                    return childHit;
            }

            return this;
        }

        public void Update(ref IElement hoverElement)
        {
            if (!Active) return;
            
            var wasHovering = IsHovering;
            // Set our hovering to whether or not the mouse is intersecting our render rectangle.
            IsHovering = MouseState.WindowBounds.Intersects(_renderRectangle) && CanInteractWithThisPieceOfShit;
            
            // We're going to head through the children NOW.
            // We pass a reference to the hover element. This way if the hover element changes in any of our children
            // we can see that, as it is a reference. TODO Mathias: SEE REFERENCE TYPES VS VALUE TYPES : https://www.tutorialsteacher.com/csharp/csharp-value-type-and-reference-type
            if (Children != null)
            {
                for(var i = Children.Count - 1; i >= 0; i--)
                    Children[i].Update(ref hoverElement);
            }

            // If we're hovering we do this.
            // This is a concise way of saying if the hover element isn't null, then it stays a hover element otherwise we set it to this element.
            // basically, if(hoverelement != null) hoverelement = hoverelement; else hoverelement = this;
            if(IsHovering)
                hoverElement = hoverElement ?? this; 

            // Now we can check if we are the hover element, and set our fill colour accordingly, otherwise just set it to a different
            // value, so we can see it working.
            if (hoverElement == this)
            {
                
                if(IsHovering && !wasHovering)
                    OnEnter?.Invoke(this);
                
                OnHover?.Invoke(this);

                if (MouseState.CurrentState.LeftButton == ButtonState.Pressed && MouseState.LastState.LeftButton == ButtonState.Released)
                {
                    OnClicked?.Invoke(this);
                }
            }
            else if (wasHovering)
            {
                OnLeave?.Invoke(this);
            }
        }

        public bool CanInteractWithThisPieceOfShit { get; set; } = true;

        public void Draw()
        {
            if (!Active) return;
            
            CalculateRenderRectangle();
            GraphicsUtils.Instance.DrawFilledRectangle(RenderRectangle.X, RenderRectangle.Y, Width, Height, FillColor);
            DrawElement();
            if (Children == null) return;

            foreach (var element in Children) element.Draw();
        }

        public abstract void DrawElement();

        /// <summary>
        /// We use this method to recalculate the position of this element in relation
        /// to its parent if it exists. If the parent DOESN'T exist then I guess we're already in the
        /// right position and we can just move on.
        /// It should take in to account our offset, so we can ideally position an element
        /// within another element at a certain offset.
        /// </summary>
        public void CalculateRenderRectangle()
        {
            var siblingOffset = 0;
            
            var newX = X;
            var newY = Y;
            var newWidth = Width;
            var newHeight = Height;

            CalculateElementSize(ref newWidth, ref newHeight);
            
            if (Parent != null)
            {
                if (Anchor == Anchor.Auto)
                {
                    var sibling = GetPreviousSibling();
                    if (sibling != null)
                    {
                        siblingOffset = (int) (sibling.RenderRectangle.Height + sibling.RelativePosition.Y);
                    }
                }

                newX = Parent.RenderRectangle.X;
                newY = Parent.RenderRectangle.Y;
            }
            else
            {
                newWidth = (int) (Window.WindowWidth - (Offset.X * 2));
                newHeight = (int) (Window.WindowHeight - (Offset.Y * 2));
            }

            X = newX;
            Y = newY + siblingOffset;
            Width = newWidth;
            Height = newHeight;

            if (Parent != null)
            {
                switch (Anchor)
                {
                    case Anchor.TopLeft:
                        break;
                    case Anchor.TopCenter:
                        newX = Parent.RenderRectangle.Width / 2 - _renderRectangle.Width / 2 + Parent.RenderRectangle.X;
                        break;
                    case Anchor.TopRight:
                        newX = (int) (Parent.RenderRectangle.Width - _renderRectangle.Width + Parent.RelativePosition.X);
                        break;
                    case Anchor.CenterLeft:
                        newY = Parent.RenderRectangle.Height / 2 - _renderRectangle.Height / 2 + (int)Parent.RelativePosition.Y;
                        break;
                    case Anchor.Center:
                        newX = Parent.RenderRectangle.Width / 2 - _renderRectangle.Width / 2 + Parent.RenderRectangle.X;
                        newY = Parent.RenderRectangle.Height / 2 - _renderRectangle.Height / 2 + Parent.RenderRectangle.Y;
                        break;
                    case Anchor.CenterRight:
                        newX = (int) (Parent.RenderRectangle.Width - _renderRectangle.Width + Parent.RelativePosition.X);
                        newY = Parent.RenderRectangle.Height / 2 - _renderRectangle.Height / 2 + Parent.RenderRectangle.Y;
                        break;
                    case Anchor.BottomLeft:
                        newY = (int) (Parent.RenderRectangle.Height - _renderRectangle.Height + Parent.RelativePosition.Y);
                        break;
                    case Anchor.BottomCenter:
                        newX = Parent.RenderRectangle.Width / 2 - _renderRectangle.Width / 2 + Parent.RenderRectangle.X;
                        newY = (int) (Parent.RenderRectangle.Height - _renderRectangle.Height + Parent.RelativePosition.Y);
                        break;
                    case Anchor.BottomRight:
                        newX = (int) (Parent.RenderRectangle.Width - _renderRectangle.Width + Parent.RelativePosition.X);
                        newY = (int) (Parent.RenderRectangle.Height - _renderRectangle.Height + Parent.RelativePosition.Y);
                        break;
                    case Anchor.Auto:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _renderRectangle.X = (int) (newX + Offset.X);
            _renderRectangle.Y = (int) (newY + Offset.Y);
            _renderRectangle.Width = Width;
            _renderRectangle.Height = Height;

            RelativePosition = Parent != null
                ? new Vector2(RenderRectangle.X, RenderRectangle.Y) -
                  new Vector2(Parent.RenderRectangle.X, Parent.RenderRectangle.Y)
                : new Vector2(0, 0);
        }

        /// <summary>
        /// Calculates the width and height of this element dependant on its size vector.
        /// </summary>
        /// <param name="width">The width of the element.</param>
        /// <param name="height">The height of the element.</param>
        private void CalculateElementSize(ref int width, ref int height)
        {
            if (Size.X > 1) width = (int) Size.X;
            else if (Size.X >= 0 && Size.X <= 1)
            {
                if (Parent != null)
                {
                    width = (int) (Parent.RenderRectangle.Width * Size.X);
                }
            }
            
            if (Size.Y > 1) height = (int) Size.Y;
            else if (Size.X >= 0 && Size.X <= 1)
            {
                if (Parent != null)
                {
                    height = (int) (Parent.RenderRectangle.Height * Size.Y);
                }
            }
        }

        /// <summary>
        /// So we can use this to get the last sibling of this element to position it in accordance to others.
        /// Say we have a list of buttons and this is the second button in the list, this should return the first button,
        /// which can allow us to use that button to then position this one.
        /// </summary>
        /// <returns>An IElement relating to the last sibling.</returns>
        public IElement GetPreviousSibling()
        {
            // No parent? No Sibling :(
            if (Parent == null) return null;

            // Keep a reference to the last checked child of the parent,
            // Just in case we're looping and we find us, we need to get the previous.
            IElement lastSibling = null;

            // Just a quick iteration over the parent to find our older sibling.
            // If the child is currently US, then the last element we checked is obviously our
            // older sibling, therefore we can break and use the one we set.
            foreach (var child in Parent.Children)
            {
                if (child == this) break;
                lastSibling = child;
            }

            return lastSibling;
        }

        public virtual void DrawDebug()
        {
            GraphicsUtils.Instance.Begin(false);
            GraphicsUtils.Instance.DrawRectangle(RenderRectangle.X, RenderRectangle.Y, RenderRectangle.Width,
                RenderRectangle.Height, Color.Red);
            GraphicsUtils.Instance.End();
            if (Children == null) return;

            foreach (var element in Children) element.DrawDebug();
        }
    }
}