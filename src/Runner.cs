using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Booster
{
    public class Runner
    {
        private readonly BoosterConfiguration _boosterConfiguration;

        public Runner(BoosterConfiguration boosterConfiguration)
        {
            _boosterConfiguration = boosterConfiguration;
        }

        public async Task RunAsync()
        {
            //TODO:check topics exist
            //TODO:check queues exist

            await StartListening();
        }

        private async Task StartListening()
        {
            var sqsClient = _boosterConfiguration.ServiceProvider.GetRequiredService<IAmazonSQS>();
            var subscriptionTasks = new List<Task>();

            foreach (var type in _boosterConfiguration.SqsSubscriptionTypes)
            {
                try
                {
                    var subscriptionGenericType = typeof(ISqsSubscriptionConfiguration<>);

                    var subscriptionSpecificType = subscriptionGenericType.MakeGenericType(type);

                    var sqsSubscriptionService =
                        _boosterConfiguration.ServiceProvider.GetRequiredService(subscriptionSpecificType);

                    var subscriptionFunc =
                        subscriptionSpecificType.GetProperties().Single(x => x.Name == "SubscriptionAsyncFunc");

                    var func =
                        (Func<IAmazonSQS, BoosterConfiguration, Task>)subscriptionFunc.GetValue(
                            sqsSubscriptionService);

                    subscriptionTasks.Add(func.Invoke(sqsClient, _boosterConfiguration));
                }
                catch (Exception ex)
                {
                    //TODO: unsubscribe?
                    throw;
                }

                await Task.WhenAll(subscriptionTasks);
            }
        }
    }
}