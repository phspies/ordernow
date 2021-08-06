
using order_microservice.Datamodels;

namespace order_microservice.Kafka
{
    [MessageTopic("ordernow-customer-events")]
    public class CustomerMessage : IMessage
    {
        public CustomerMessage(CustomerKafkaMessage _customerObject)
        {
            customerObject = _customerObject;
        }

        public string Key { get; }

        public CustomerKafkaMessage customerObject { get; }
    }
}