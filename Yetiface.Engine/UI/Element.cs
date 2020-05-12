﻿using System.Collections.Generic;
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

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        ///  The constructor should currently just create a new render rectangle from the X, Y, Width and Height
        /// that have hopefully been set as properties.
        /// </summary>
        protected Element(int offsetX, int offsetY)
        {
            Offset = new Vector2(offsetX, offsetY);
        }

        /// <summary>
        ///  The render rectangle will keep track of the actual render position of the element, and should be updated
        ///  in regards to the elements parent.
        /// </summary>
        private Rectangle _renderRectangle;

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
            CalculateRectangle();

            if (Children == null) return;

            foreach (var element in Children) element.Draw();
        }

        /// <summary>
        /// We use this method to recalculate the position of this element in relation
        /// to its parent if it exists. If the parent DOESN'T exist then I guess we're already in the
        /// right position and we can just move on.
        /// It should take in to account our offset, so we can ideally position an element
        /// within another element at a certain offset.
        /// </summary>
        public void CalculateRectangle()
        {
            var newX = X;
            var newY = Y;
            var newWidth = Width;
            var newHeight = Height;

            if (Parent != null)
            {
                newX = Parent.X;
                newY = Parent.Y;
                newWidth = (int) (Parent.Width - (Offset.X * 2));
                newHeight = (int) (Parent.Height - (Offset.Y * 2));
            }

            X = newX;
            Y = newY;
            Width = newWidth;
            Height = newHeight;

            _renderRectangle.X = (int) (X + Offset.X);
            _renderRectangle.Y = (int) (Y + Offset.Y);
            _renderRectangle.Width = Width;
            _renderRectangle.Height = Height;
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
            GraphicsUtils.Instance.DrawRectangle(_renderRectangle.X, _renderRectangle.Y, _renderRectangle.Width,
                _renderRectangle.Height, Color.Red);

            if (Children == null) return;

            foreach (var element in Children) element.DrawDebug();
        }
    }
}