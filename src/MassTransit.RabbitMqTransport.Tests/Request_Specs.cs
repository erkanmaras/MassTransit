﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [TestFixtureSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8));

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_missing_service :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_timeout()
        {
            Assert.Throws<RequestTimeoutException>(async () => await _response);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [TestFixtureSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(1));

            _response = _requestClient.Request(new PingMessage());
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_faulty_service :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_receive_the_exception()
        {
            Assert.Throws<RequestFaultException>(async () => await _response);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [TestFixtureSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8));

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                throw new InvalidOperationException("This is an expected test failure");
            });
        }
    }
}