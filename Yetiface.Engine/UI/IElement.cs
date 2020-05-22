using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yetiface.Engine.UI
{

    /// <summary>
    /// Auto is used when stacking elements. Left center and right will just use the parent and not the siblings to
    /// position an element.
    /// TODO WE REALLY NEED MORE ANCHORS, TOP LEFT, TOP CENTER, TOP RIGHT, CENTER LEFT, CENTER, CENTER RIGHT, BOTTOM LEFT, BOTTOM CENTER, BOTTOM RIGHT
    /// because we need to be able to anchor stuff so that any resolution changes doesnt affect the positioning of UI elements. :) :)
    /// </summary>
    public enum Anchor
    {
        Auto,
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    
    public interface IElement
    {
        IUserInterface UserInterface { get; set; }
        IElement Parent { get; set; }
        IList<IElement> Children { get; set; }
        Vector2 Offset { get; set; }
        
        /// <summary>
        /// Controls the relative size to the parent, 1 indicates full parent size on the relevant axis.
        /// Numbers larger than 1 indicate explicit size.
        /// Anything between 0 and 1 will indicate a certain size relative to the parents.
        /// </summary>
        Vector2 Size { get; set; }

        /// <summary>
        /// The position relative to the element' parent
        /// </summary>
        Vector2 RelativePosition { get; set; }

        /// <summary>
        /// The anchor will be where the element holds its children.
        /// </summary>
        Anchor Anchor { get; set; }
        
        int X { get; set; }
        int Y { get; set; }
        int Width { get; }
        int Height { get; }

        Color FillColor { get; set; }

        Rectangle RenderRectangle { get; }

        bool IsHovering { get; set; }

        // This generic basically says, Add an element of some type, as long as that type is an IElement.
        // Then it can return the type that we pass in. This lets us have access to any properties that are only
        // on a certain type of element. For instance, if the element is a button, we may want to have access
        // to certain properties, such as an OnClick event we can register to.
        // TODO Mathias: SEE DELEGATES: https://www.reddit.com/r/explainlikeimfive/comments/1k0tk5/eli5_what_are_delegates_in_c_how_do_they_work_and/
        // TODO Mathias: SEE GENERICS: https://www.tutorialspoint.com/csharp/csharp_generics.htm
        T AddElement<T>(T element) where T : IElement;

        void CalculateRenderRectangle();
        IElement GetPreviousSibling();
        IElement GetElementUnderMouse(Rectangle mouseBounds);

        void Update(ref IElement hoverElement);
        void Draw();

        /// <summary>
        /// Element specific draw method, override this and call base.
        /// It will be called in the elements Draw method.
        /// </summary>
        void DrawElement();
        void DrawDebug();
    }
}