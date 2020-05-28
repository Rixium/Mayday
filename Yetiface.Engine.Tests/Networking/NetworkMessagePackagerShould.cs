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
            
            networkMessagePackager.AddDefinition<TestPacket>();

            networkMessagePackager.GetPacketDefinition(0)
                .PacketType.ShouldBe(typeof(TestPacket));
        }
        
        [Test]
        public void CorrectlyIncrementPacketTypeIds()
        {
            var networkMessagePackager = new NetworkMessagePackager();
            
            var first = networkMessagePackager.AddDefinition<TestPacket>();
            var second = networkMessagePackager.AddDefinition<TestPacket>();

            first.PacketTypeId.ShouldNotBe(second.PacketTypeId);
            second.PacketTypeId.ShouldBeGreaterThan(first.PacketTypeId);
        }

        [Test]
        public void PackageCorrectlyBasedOnType()
        {
            var networkMessagePackager = new NetworkMessagePackager();
            
            var testPacket = new TestPacket()
            {
                Number = 3,
                Name = "Test"
            };

            var definition = networkMessagePackager.AddDefinition<TestPacket>();

            var value = networkMessagePackager.Package(testPacket);
            
            var expectedString = $"{definition.PacketTypeId}:{testPacket.Number}:{testPacket.Name}";
            var outputString = Encoding.UTF8.GetString(value);
            
            outputString.ShouldBe(expectedString);
        }

        [Test]
        public void UnPackCorrectly()
        {
            var networkMessagePackager = new NetworkMessagePackager();
            
            var testPacket = new TestPacket()
            {
                Number = 3,
                Name = "Test"
            };

            networkMessagePackager.AddDefinition<TestPacket>();
            
            var value = networkMessagePackager.Package(testPacket);
            var result = (TestPacket) networkMessagePackager.Unpack(value);
            
            result.Name.ShouldBe(testPacket.Name);
            result.Number.ShouldBe(testPacket.Number);
        }
        
    }

    public class TestPacket : INetworkPacket
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }
    
}