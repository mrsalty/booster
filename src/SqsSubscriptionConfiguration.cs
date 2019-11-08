using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;

namespace Booster
{
    public class SqsSubscriptionConfiguration
    {
        public int MaxNumberOfMessages { get; set; }

        public int WaitTimeSeconds { get; set; }

        public int VisibilityTimeout { get; set; }

        public string QueueUrl { get; set; }

    }
}