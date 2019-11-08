using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Booster
{
    public class Boost
    {
        private readonly BoosterConfiguration _boosterConfiguration;

        public Boost(BoosterConfiguration boosterConfiguration)
        {
            _boosterConfiguration = boosterConfiguration;
        }

        public async Task RunAsync()
        {
            //TODO:check publish topics exists
            //TODO:check subscribing queues exists
            //TODO:parallele listening to subscriptions
            var sqsClient = _boosterConfiguration.ServiceProvider.GetRequiredService<IAmazonSQS>();
            
            foreach (var keyValuePair in _boosterConfiguration.SqsSubscriptionFuncsDictionary)
            {
                try
                {
                    var sqsSubscriptionConfiguration = _boosterConfiguration.SqsSubscriptions.Single(x => x.Key.Equals(keyValuePair.Key)).Value;
                    await keyValuePair.Value.Invoke(sqsSubscriptionConfiguration, sqsClient, _boosterConfiguration);
                }

                catch (Exception ex)
                {
                    //TODO: unsubscribe?
                }
            }
        }
    }
}