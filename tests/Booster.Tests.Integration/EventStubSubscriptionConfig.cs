namespace Booster.Tests.Integration
{
    public class EventStubSubscriptionConfig
    {
        public int MaxNumberOfMessages { get; set; }

        public int WaitTimeSeconds { get; set; }

        public int VisibilityTimeout { get; set; }

        public string QueueUrl { get; set; }
    }
}