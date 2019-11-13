using Microsoft.Extensions.Options;

namespace Booster.Tests.Integration
{
    public class EventStubSqsConfiguration : SqsSubscriptionConfiguration<EventStub>
    {
        public EventStubSqsConfiguration(IOptions<EventStubSubscriptionConfig> configuration)
        {
            QueueUrl = configuration.Value.QueueUrl;
            MaxNumberOfMessages = configuration.Value.MaxNumberOfMessages;
            VisibilityTimeout = configuration.Value.VisibilityTimeout;
            WaitTimeSeconds = configuration.Value.WaitTimeSeconds;
        }
    }
}