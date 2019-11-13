using System;
using System.Collections.Generic;

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

        public List<Type> SqsSubscriptionTypes { get; }

        public BoosterConfiguration(IServiceProvider serviceProvider, List<Type> sqsSubscriptionTypes)
        {
            ServiceProvider = serviceProvider;
            SqsSubscriptionTypes = sqsSubscriptionTypes;
        }
    }
}
