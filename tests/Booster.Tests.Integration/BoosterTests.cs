using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using Xunit;

namespace Booster.Tests.Integration
{
    public class BoosterTests
    {
        private readonly IConfigurationRoot _configuration;

        public BoosterTests()
        {
            _configuration = BuildConfiguration();
        }

        [Fact]
        public async Task HandlesAsync_WhenEventInSqsQueue_ThenHandlerHandlesEvent()
        {
            //arrange
            var @event1 = CreateEventStub(Guid.NewGuid().ToString("N"));
            var @event2 = CreateEventStub(Guid.NewGuid().ToString("N"));
            var @event3 = CreateEventStub(Guid.NewGuid().ToString("N"));

            await Task.WhenAll(
                PublishEventStubToQueue(@event1),
                PublishEventStubToQueue(@event2),
                PublishEventStubToQueue(@event3)
            );

            //act
            RunBooster();

            //assert
            Assert.True(
                Policy
                    .HandleResult(false)
                    .WaitAndRetry(3, i => TimeSpan.FromSeconds(2))
                    .Execute(() =>
                        TestDomainContext.ProcessedEvents.Contains(JsonConvert.DeserializeObject<EventStub>(@event1)) &&
                        TestDomainContext.ProcessedEvents.Contains(JsonConvert.DeserializeObject<EventStub>(@event2)) &&
                        TestDomainContext.ProcessedEvents.Contains(JsonConvert.DeserializeObject<EventStub>(@event3))));
        }

        private static string CreateEventStub(string eventId)
        {
            return "{\"id\" : \"" + eventId + "\", \"message\" : \"hello there!\"}";
        }

        private async Task PublishEventStubToQueue(string @event)
        {
            var awsProfile = _configuration.GetSection("AWS").GetSection("Profile").Value;
            var testQueueUrl = _configuration.GetSection("EventStubSubscription").GetSection("QueueUrl").Value;
            var credentials = new StoredProfileAWSCredentials(awsProfile);
            var sqsClient = new AmazonSQSClient(new AnonymousAWSCredentials(), new AmazonSQSConfig()
            {
                ServiceURL = _configuration.GetSection("AWS").GetSection("ServiceUrl").Value
            });

            await sqsClient.SendMessageAsync(new SendMessageRequest()
            {
                QueueUrl = testQueueUrl,
                MessageBody = @event
            });
        }

        private void RunBooster()
        {
            var testQueueUrl = _configuration.GetSection("EventStubSubscription").GetSection("QueueUrl").Value;

            var serviceCollection = new ServiceCollection()
                .AddDefaultAWSOptions(_configuration.GetAWSOptions())
                .AddTransient<ISqsSubscriptionConfiguration<EventStub>, EventStubSqsConfiguration>()
                .AddAWSService<IAmazonSQS>(new AWSOptions()
                {
                    Credentials = new AnonymousAWSCredentials(),
                    Profile = _configuration.GetSection("AWS").GetSection("Profile").Value,
                })
                .AddOptions();

            serviceCollection.Configure<EventStubSubscriptionConfig>(_configuration.GetSection("EventStubSubscription"));

            var boosterConfiguration = new BoosterConfigurationBuilder(serviceCollection)
                .HandlesEvent<EventStub, EventStubHandler, EventStubSqsConfiguration>()
                .Build();

            Task.Run(() => new Runner(boosterConfiguration).RunAsync());
        }

        public static IConfigurationRoot BuildConfiguration()
        {
            var environmentConfig = new ConfigurationBuilder().Build();
            var environmentName = environmentConfig.GetValue<string>("NETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }

    public static class TestDomainContext
    {
        public static ConcurrentBag<IEvent> ProcessedEvents = new ConcurrentBag<IEvent>();
    }

    public class EventStubHandler : IHandlerAsync<EventStub>
    {
        public Task HandleAsync(EventStub @event)
        {
            TestDomainContext.ProcessedEvents.Add(@event);
            return Task.CompletedTask;
        }
    }

    public class EventStub : IEvent, IEquatable<EventStub>
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public bool HaveBeenProcessed { get; set; }

        public bool Equals(EventStub other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id) && string.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventStub)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Message);
        }
    }
}
