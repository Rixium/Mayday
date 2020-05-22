using NUnit.Framework;
using Shouldly;
using Yetiface.Engine.UI;
using Yetiface.Engine.UI.Widgets;

namespace Yetiface.Engine.Tests.UI
{
    public class ElementShould
    {
        
        [Test]
        public void HaveParentWhenAddingToElement()
        {
            var panel = new Panel();

            var child1 = panel.AddElement(new Panel());
            
            child1.Parent.ShouldBe(panel);
        }
        
        [Test]
        public void GetItsPreviousSibling()
        {
            var element = new Panel();

            var childPanel1 = element.AddElement(new Panel());
            var childPanel2 = element.AddElement(new Panel());
            var childPanel3 = element.AddElement(new Panel());
            
            childPanel2.GetPreviousSibling().ShouldBe(childPanel1);
            childPanel3.GetPreviousSibling().ShouldBe(childPanel2);
        }
        
        
        [Test]
        public void HaveNoParentIfFirstElement()
        {
            var userInterface = new UserInterface();
            userInterface.Root.Parent.ShouldBeNull();
        }
        
    }
}