using Mayday.Game.Screens;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Mayday.Game.Tests.Screens
{
    [TestFixture]
    public class ScreenManagerShould
    {
        
        private const string ScreenName = "MockScreen";
        
        [Test]
        public void AddScreenToDictionaryByName()
        {
            var screenManager = new ScreenManager();
            var screen = Substitute.For<IScreen>();
            screen.Name.Returns(ScreenName);
            
            screenManager.AddScreen(screen);
            
            screenManager.GetScreen(ScreenName).ShouldBe(screen);
        }
    }
}