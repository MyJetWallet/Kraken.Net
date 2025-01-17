﻿using CryptoExchange.Net;
using Kraken.Net.UnitTests.TestImplementations;
using Kucoin.Net.UnitTests.TestImplementations;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

using System.Threading.Tasks;

namespace Kraken.Net.UnitTests
{
    [TestFixture]
    public class KrakenSocketClientTests
    {
        [Test]
        public async Task Subscribe_Should_SucceedIfAckResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            // act
            var subTask = client.SpotStreams.SubscribeToTickerUpdatesAsync("XBT/EUR", test => { });
            await Task.Delay(10);
            var id = JToken.Parse(socket.LastSendMessage!)["reqid"];
            socket.InvokeMessage($"{{\"channelID\": 1, \"status\": \"subscribed\", \"reqid\":{id}}}");
            var subResult = subTask.Result;

            // assert
            ClassicAssert.IsTrue(subResult.Success);
        }

        [Test]
        public async Task Subscribe_Should_FailIfNotAckResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            // act
            var subTask = client.SpotStreams.SubscribeToTickerUpdatesAsync("XBT/EUR", test => { });
            await Task.Delay(10);
            var id = JToken.Parse(socket.LastSendMessage!)["reqid"];
            socket.InvokeMessage($"{{\"channelID\": 1, \"status\": \"error\", \"errormessage\": \"Failed to sub\", \"reqid\":{id}}}");
            var subResult = subTask.Result;

            // assert
            ClassicAssert.IsFalse(subResult.Success);
            ClassicAssert.IsTrue(subResult.Error!.Message.Contains("Failed to sub"));
        }
    }
}
