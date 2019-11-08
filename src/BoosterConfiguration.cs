using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;

namespace Booster
{
    //TODO
    //list of queues to subscribe => for each configuration?//
    //list of topics to publish => for each configuration?
    //naming convention
    //aws credentials (allow goaws)
    //aws region
    //logging
    //DI
    public class BoosterConfiguration
    {
        public IServiceProvider ServiceProvider { get; }

        public Dictionary<string, SqsSubscriptionConfiguration> SqsSubscriptions { get; }

        public Dictionary<string, Func<SqsSubscriptionConfiguration, IAmazonSQS, BoosterConfiguration, Task>> SqsSubscriptionFuncsDictionary { get; }

        public BoosterConfiguration(IServiceProvider serviceProvider,
            Dictionary<string, SqsSubscriptionConfiguration> sqsSubscriptions,
            Dictionary<string, Func<SqsSubscriptionConfiguration, IAmazonSQS, BoosterConfiguration, Task>> sqsSubscriptionFuncs
        )
        {
            ServiceProvider = serviceProvider;
            SqsSubscriptions = sqsSubscriptions;
            SqsSubscriptionFuncsDictionary = sqsSubscriptionFuncs;
        }
    }
}
