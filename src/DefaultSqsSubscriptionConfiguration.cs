namespace Booster
{
    public class DefaultSqsSubscriptionConfiguration<T> : SqsSubscriptionConfiguration<T> where T : IEvent
    {
        public const int DefaultWaitTimeSeconds = 5;

        public const int DefaultVisibilityTimeOut = 30;

        public const int DefaultMaxNumberOfMessages = 10;

        public DefaultSqsSubscriptionConfiguration(string queueUrl)
        {
            QueueUrl = queueUrl;
            MaxNumberOfMessages = DefaultMaxNumberOfMessages;
            VisibilityTimeout = DefaultVisibilityTimeOut;
            WaitTimeSeconds = DefaultWaitTimeSeconds;
        }
    }
}