using NUnit.Framework;
using Shouldly;
using Yetiface.Engine.UI;
using Yetiface.Engine.UI.Widgets;

namespace Yetiface.Engine.Tests.UI
{
    public class UserInterfaceShould
    {

        [Test]
        public void SetRootWhenAddingAnElement()
        {
            var userInterface = new UserInterface();

            var panel = userInterface.SetRoot(new Panel());
            
            userInterface.Root.ShouldBe(panel);
        }
    }
}