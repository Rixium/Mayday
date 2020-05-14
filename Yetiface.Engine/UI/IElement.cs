using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yetiface.Engine.UI
{
    public interface IElement
    {
        IUserInterface UserInterface { get; set; }
        IElement Parent { get; set; }
        IList<IElement> Children { get; set; }
        Vector2 Offset { get; set; }

        /// <summary>
        /// The position relative to the element' parent
        /// </summary>
        Vector2 RelativePosition { get; set; }

        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        Color FillColor { get; set; }

        Rectangle RenderRectangle { get; }

        bool FillToParent { get; set; }
        bool IsHovering { get; set; }

        // This generic basically says, Add an element of some type, as long as that type is an IElement.
        // Then it can return the type that we pass in. This lets us have access to any properties that are only
        // on a certain type of element. For instance, if the element is a button, we may want to have access
        // to certain properties, such as an OnClick event we can register to.
        // TODO Mathias: SEE DELEGATES: https://www.reddit.com/r/explainlikeimfive/comments/1k0tk5/eli5_what_are_delegates_in_c_how_do_they_work_and/
        // TODO Mathias: SEE GENERICS: https://www.tutorialspoint.com/csharp/csharp_generics.htm
        T AddElement<T>(T element) where T : IElement;

        void CalculateRectangle();
        IElement GetPreviousSibling();
        IElement GetElementUnderMouse(Rectangle mouseBounds);

        void Update(ref IElement hoverElement);
        void Draw();
        void DrawDebug();
    }
}