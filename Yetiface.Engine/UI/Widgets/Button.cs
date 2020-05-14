using System;
using Microsoft.Xna.Framework;

namespace Yetiface.Engine.UI.Widgets
{
    public class Button : Element
    {

        // This is an action which is a type of delegate. We probably don't want that here, but rather in element
        // eventually, since perhaps all elements want to fire an on click event if it's clicked.
        // TODO Mathias: SEE DELEGATES: https://www.reddit.com/r/explainlikeimfive/comments/1k0tk5/eli5_what_are_delegates_in_c_how_do_they_work_and/
        public Action OnClicked;
        
        public Button(int offsetX = 0, int offsetY = 0) : base(offsetX, offsetY)
        {
            FillColor = Color.Black * 0.7f;
        }
    }
}