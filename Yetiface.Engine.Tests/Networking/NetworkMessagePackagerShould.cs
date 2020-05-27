using System;
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

            networkMessagePackager.GetPacketDefinition(0).ShouldBe(testPacketDefinition);
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

            var expectedString = $"{testPacketDefinition.PacketTypeId}:{testPacket.Name}:{testPacket.Number}";
            var outputString = Encoding.UTF8.GetString(value);

            outputString.ShouldBe(expectedString);
        }

        [Test]
        public void UnPackCorrectly()
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

    public class TestPacketDefinition : PacketDefinition
    {
        public override string Pack(INetworkPacket data)
        {
            var value = (TestPacket) data;
            return value.Name + ":" + value.Number;
        }

        public override INetworkPacket Unpack(string data)
        {
            var splitData = data.Split(':');
            return new TestPacket()
            {
                Name = splitData[0],
                Number = int.Parse(splitData[1])
            };
        }
        
    }
    
}