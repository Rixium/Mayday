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
        
        private IScreenManager _screenManager;
        private IScreen _screen;
        
        [SetUp]
        public void SetUp()
        {
            _screenManager = new ScreenManager();
            _screen = Substitute.For<IScreen>();
            _screen.Name.Returns(ScreenName);
        }
        
        [Test]
        public void AddScreenToDictionaryByName()
        {
            _screenManager.AddScreen(_screen);
            
            _screenManager.Screens[_screen.Name].ShouldBe(_screen);
        }

        [Test]
        public void RetrieveScreenByName()
        {
            _screenManager.AddScreen(_screen);

            _screenManager.GetScreen(_screen.Name).ShouldBe(_screen);
        }

        [Test]
        public void GetNullIfScreenNotExist()
        {
            _screenManager.GetScreen(_screen.Name).ShouldBeNull();
        }

        [Test]
        public void RemoveScreenByName()
        {
            _screenManager.AddScreen(_screen);
            _screenManager.RemoveScreen(_screen.Name);
            
            _screenManager.Screens.ShouldNotContainKey(_screen.Name);
        }
        
        [Test]
        public void RemoveScreen()
        {
            _screenManager.AddScreen(_screen);
            _screenManager.RemoveScreen(_screen);

            _screenManager.Screens.ShouldNotContainKey(_screen.Name);
        }
        
    }
}