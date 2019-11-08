using System;

namespace Booster
{
    public class SqsPathBuilder : PathBuilderBase
    {
        private readonly string _environmentPrefix;
        private const int AwsQueueNamecMaxLength = 80;

        public SqsPathBuilder(string environmentPrefix)
        {
            _environmentPrefix = environmentPrefix;
        }

        public override string GetPathFor(Type type)
        {
            string queueName = CleanUp($"{_environmentPrefix}-{GetTypeName(type)}");
            return queueName.Length > AwsQueueNamecMaxLength
                ? queueName.Substring(0, AwsQueueNamecMaxLength)
                : queueName;
        }

        public override string GetPathFor<T>()
        {
            string queueName = CleanUp($"{_environmentPrefix}-{GetTypeName(typeof(T))}");
            return queueName.Length > AwsQueueNamecMaxLength
                ? queueName.Substring(0, AwsQueueNamecMaxLength)
                : queueName;
        }
    }
}