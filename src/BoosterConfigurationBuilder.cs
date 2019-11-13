using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Booster
{
    public class BoosterConfigurationBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;
        private readonly List<Type> _sqsSubscriptionTypes;

        public BoosterConfigurationBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _sqsSubscriptionTypes = new List<Type>();
        }

        public BoosterConfiguration Build()
        {
            if (_serviceProvider == null)
            {
                _serviceProvider = _serviceCollection.BuildServiceProvider();
            }

            return new BoosterConfiguration(_serviceProvider, _sqsSubscriptionTypes);
        }

        public BoosterConfigurationBuilder WithServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public BoosterConfigurationBuilder HandlesEvent<TEvent, THandler, TConfig>()
            where TEvent : IEvent
            where THandler : class, IHandlerAsync<TEvent>
            where TConfig : class, ISqsSubscriptionConfiguration<TEvent>
        {
            _serviceCollection.AddTransient<IHandlerAsync<TEvent>, THandler>();
            _serviceCollection.AddTransient<ISqsSubscriptionConfiguration<TEvent>, TConfig>();
            _sqsSubscriptionTypes.Add(typeof(TEvent));
            return this;
        }
    }
}