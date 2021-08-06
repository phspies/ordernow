using System;

namespace order_microservice.Kafka
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageTopicAttribute : Attribute
    {
        public MessageTopicAttribute(string topic)
        {
            Topic = topic;
        }

        public string Topic { get; }
    }
}