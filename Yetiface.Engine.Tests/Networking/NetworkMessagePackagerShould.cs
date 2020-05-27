using System.Text;
using NUnit.Framework;
using Shouldly;
using Yetiface.Engine.Networking.Packagers;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Tests.Networking
{
    public class NetworkMessagePackagerShould
    {
        [Test]
        public void AddPacketDefinitionToDictionaryCorrectly()
        {
            var networkMessagePackager = new NetworkMessagePackager();
            var testPacketDefinition = new TestPacketDefinition();

            networkMessagePackager.AddDefinition(typeof(TestPacket), testPacketDefinition);

            networkMessagePackager.GetPacketDefinition(typeof(TestPacket)).ShouldBe(testPacketDefinition);
        }

        [Test]
        public void PackageCorrectlyBasedOnType()
        {
            var networkMessagePackager = new NetworkMessagePackager();
            var testPacketDefinition = new TestPacketDefinition();
            var testPacket = new TestPacket()
            {
                Number = 3,
                Name = "Test"
            };

            networkMessagePackager.AddDefinition(typeof(TestPacket), testPacketDefinition);

            var value = networkMessagePackager.Package(testPacket);

            var expectedString = $"{testPacket.Name}:{testPacket.Number}";
            var outputString = Encoding.UTF8.GetString(value);

            outputString.ShouldBe(expectedString);
        }
    }

    public class TestPacket : INetworkPacket
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public class TestPacketDefinition : IPacketDefinition
    {
        public byte[] Create(object data)
        {
            var value = (TestPacket) data;
            var dataString = value.Name + ":" + value.Number;
            return Encoding.UTF8.GetBytes(dataString);
        }
    }
    
}