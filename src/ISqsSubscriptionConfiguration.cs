using System;
using System.Threading.Tasks;
using Amazon.SQS;

namespace Booster
{
    public interface ISqsSubscriptionConfiguration<T> where T : IEvent
    {
        int MaxNumberOfMessages { get; set; }

        int WaitTimeSeconds { get; set; }

        int VisibilityTimeout { get; set; }

        string QueueUrl { get; set; }

        Func<IAmazonSQS, BoosterConfiguration, Task> SubscriptionAsyncFunc { get; }
    }
}