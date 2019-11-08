namespace Booster
{
    public class DefaultSqsSubscriptionConfiguration : SqsSubscriptionConfiguration
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