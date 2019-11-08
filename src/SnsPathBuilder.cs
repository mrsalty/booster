using System;

namespace Booster
{
    public class SnsPathBuilder : PathBuilderBase
    {
        private readonly string _environmentPrefix;
        private const int AwsTopicNameMaxLength = 80;

        public SnsPathBuilder(string environmentPrefix)
        {
            _environmentPrefix = environmentPrefix;
        }

        public override string GetPathFor(Type type)
        {
            string topicName = CleanUp($"{_environmentPrefix}-{GetTypeName(type)}");
            return topicName.Length > AwsTopicNameMaxLength
                ? topicName.Substring(0, AwsTopicNameMaxLength)
                : topicName;
        }

        public override string GetPathFor<T>()
        {
            string topicName = CleanUp($"{_environmentPrefix}-{GetTypeName(typeof(T))}");
            return topicName.Length > AwsTopicNameMaxLength
                ? topicName.Substring(0, AwsTopicNameMaxLength)
                : topicName;
        }
    }
}