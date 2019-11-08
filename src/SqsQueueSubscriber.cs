using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Booster
{
    public class SqsQueueSubscriber
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly SqsSubscriptionConfiguration _sqsSubscriptionConfiguration;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<string> _messageAttributeNames;

        public SqsQueueSubscriber(
            IAmazonSQS sqsClient,
            IServiceProvider serviceProvider,
            SqsSubscriptionConfiguration sqsSubscriptionConfiguration,
            List<string> messageAttributeNames,
            CancellationTokenSource cancellationTokenSource)
        {
            _sqsClient = sqsClient;
            _serviceProvider = serviceProvider;
            _sqsSubscriptionConfiguration = sqsSubscriptionConfiguration;
            _messageAttributeNames = messageAttributeNames;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public void Unsubscribe()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task SqsSubscribeAsync<T>() where T : IEvent
        {
            var handler = _serviceProvider.GetService<IHandlerAsync<T>>();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var request = new ReceiveMessageRequest
                {
                    MaxNumberOfMessages = _sqsSubscriptionConfiguration.MaxNumberOfMessages,
                    WaitTimeSeconds = _sqsSubscriptionConfiguration.WaitTimeSeconds,
                    VisibilityTimeout = _sqsSubscriptionConfiguration.VisibilityTimeout,
                    QueueUrl = _sqsSubscriptionConfiguration.QueueUrl,
                    MessageAttributeNames = _messageAttributeNames
                };

                try
                {
                    var result = await _sqsClient.ReceiveMessageAsync(request, _cancellationTokenSource.Token);

                    if (result.Messages.Any())
                    {
                        foreach (var message in result.Messages)
                        {
                            try
                            {
                                await handler.HandleAsync(JsonConvert.DeserializeObject<T>(message.Body));
                            }
                            finally
                            {
                                await DeleteMessageAsync(message.ReceiptHandle);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }

        private async Task DeleteMessageAsync(string receiptHandle)
        {
            var request = new DeleteMessageRequest
            {
                QueueUrl = _sqsSubscriptionConfiguration.QueueUrl,
                ReceiptHandle = receiptHandle
            };

            await _sqsClient.DeleteMessageAsync(request);
        }
    }
}