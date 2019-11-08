using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Booster
{
    public class BoosterConfigurationBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Dictionary<string, SqsSubscriptionConfiguration> _sqsSubscriptions;
        private readonly Dictionary<string, Func<SqsSubscriptionConfiguration, IAmazonSQS, BoosterConfiguration, Task>> _sqsSubscriptionFuncs;

        public BoosterConfigurationBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _sqsSubscriptionFuncs = new Dictionary<string, Func<SqsSubscriptionConfiguration, IAmazonSQS, BoosterConfiguration, Task>>();
            _sqsSubscriptions = new Dictionary<string, SqsSubscriptionConfiguration>();
        }
        
        public BoosterConfiguration Build()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            return new BoosterConfiguration(serviceProvider, _sqsSubscriptions, _sqsSubscriptionFuncs);
        }

        public BoosterConfigurationBuilder HandlesEvent<T, TY>(SqsSubscriptionConfiguration configuration)
            where T : IEvent
            where TY : class, IHandlerAsync<T>
        {
            _serviceCollection.AddTransient<IHandlerAsync<T>, TY>();
            _sqsSubscriptions.Add(typeof(T).Name, configuration);
            _sqsSubscriptionFuncs.Add(typeof(T).Name, SubscribeAsync<T>);
            return this;
        }

        private async Task SubscribeAsync<T>(SqsSubscriptionConfiguration config, IAmazonSQS sqsClient, BoosterConfiguration boosterConfig) where T : IEvent
        {
            await new SqsQueueSubscriber(
                    sqsClient, boosterConfig.ServiceProvider, config, new List<string>(), new CancellationTokenSource())
                .SqsSubscribeAsync<T>();
        }    
    }
}