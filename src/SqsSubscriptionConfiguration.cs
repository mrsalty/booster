using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;

namespace Booster
{
    public class SqsSubscriptionConfiguration<T> : ISqsSubscriptionConfiguration<T>
        where T : IEvent
    {
        public int MaxNumberOfMessages { get; set; }

        public int WaitTimeSeconds { get; set; }

        public int VisibilityTimeout { get; set; }

        public string QueueUrl { get; set; }

        public Func<IAmazonSQS, BoosterConfiguration, Task> SubscriptionAsyncFunc => (sqsClient, config) =>
            SubscribeAsync<T>(this, sqsClient, config);

        private async Task SubscribeAsync<T>(SqsSubscriptionConfiguration<T> config, IAmazonSQS sqsClient, BoosterConfiguration boosterConfig) where T : IEvent
        {
            await new SqsQueueSubscriber<T>(
                    sqsClient, boosterConfig.ServiceProvider, config, new List<string>(), new CancellationTokenSource())
                .SqsSubscribeAsync<T>();
        }
    }
}