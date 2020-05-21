﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI
{
    public abstract class Element : IElement
    {
        private bool _debug;

        public IUserInterface UserInterface { get; set; }

        public IElement Parent { get; set; }
        public IList<IElement> Children { get; set; }

        public Vector2 Offset { get; set; }

        public Vector2 RelativePosition { get; set; }
        
        public Anchor Anchor { get; set; } = Anchor.Center;

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Color FillColor { get; set; }
        
        public Action OnHover { get; set; }

        /// <summary>
        ///  The constructor should currently just create a new render rectangle from the X, Y, Width and Height
        /// that have hopefully been set as properties.
        /// </summary>
        protected Element(int offsetX, int offsetY, bool fillToParent = true)
        {
            Offset = new Vector2(offsetX, offsetY);
            FillToParent = fillToParent;
        }

        /// <summary>
        ///  The render rectangle will keep track of the actual render position of the element, and should be updated
        ///  in regards to the elements parent.
        /// </summary>

        public Rectangle RenderRectangle => _renderRectangle;

        public bool FillToParent { get; set; }
        public bool IsHovering { get; set; }

        private Rectangle _renderRectangle;
        
        public T AddElement<T>(T element) where T : IElement
        {
            if (Children == null) Children = new List<IElement>();
            element.Parent = this;
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
            // Set our hovering to whether or not the mouse is intersecting our render rectangle.
            IsHovering = MouseState.Bounds.Intersects(_renderRectangle);
            
            // We're going to head through the children NOW.
            // We pass a reference to the hover element. This way if the hover element changes in any of our children
            // we can see that, as it is a reference. TODO Mathias: SEE REFERENCE TYPES VS VALUE TYPES : https://www.tutorialsteacher.com/csharp/csharp-value-type-and-reference-type
            if (Children != null)
            {
                foreach (var element in Children) 
                    element.Update(ref hoverElement);
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
                OnHover?.Invoke();
            }
            else
            {
                
            }
        }

        public void Draw()
        {
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
            
            if (Anchor == Anchor.Auto)
            {
                var sibling = GetPreviousSibling();
                if (sibling != null)
                {
                    siblingOffset = sibling.RenderRectangle.Height + (int) sibling.RelativePosition.Y;
                }
            }

            var newX = X;
            var newY = Y;
            var newWidth = Width;
            var newHeight = Height;

            if (Parent != null)
            {
                newX = Parent.RenderRectangle.X;
                newY = Parent.RenderRectangle.Y;

                if (FillToParent)
                {
                    newWidth = (int) (Parent.RenderRectangle.Width - (Offset.X * 2));
                    newHeight = (int) (Parent.RenderRectangle.Height - (Offset.Y * 2));
                }
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

            // We can finally apply our offset to our Y.

            newY = (int) (Y + Offset.Y);

            // TODO this is fucking gross, and needs a refactor, but it is something that needs to happen
            // since the render rectangle should be affected by the anchor of the parent.
            if (Parent != null)
            {
                switch (Parent.Anchor)
                {
                    case Anchor.Left:
                        newX = Parent.RenderRectangle.X + (int) Offset.X;
                        break;
                    case Anchor.Center:
                        // I guess the center is the parents render x, plus half of its width, this should get our
                        // left side lined up with the center of the parents render rectangle.
                        // At that point we can just also take off half of our width, so that we line up in the center.
                        newX = (int) (Parent.RenderRectangle.X + Parent.RenderRectangle.Width / 2.0f -
                                      RenderRectangle.Width / 2.0f);
                        break;
                    case Anchor.Right:
                        // The right should set us completely on the right side of the parents render rectangle using
                        // parent render x + parent render width then we can take away our width, and we should have
                        // our right side lined up with the parents right side.
                        newX = Parent.RenderRectangle.X + Parent.RenderRectangle.Width - (int) (RenderRectangle.Width + Offset.X);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                newX = (int) (X + Offset.X);
            }

            _renderRectangle.X = newX;
            _renderRectangle.Y = newY;
            _renderRectangle.Width = Width;
            _renderRectangle.Height = Height;

            RelativePosition = Parent != null
                ? new Vector2(RenderRectangle.X, RenderRectangle.Y) -
                  new Vector2(Parent.RenderRectangle.X, Parent.RenderRectangle.Y)
                : new Vector2(0, 0);
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