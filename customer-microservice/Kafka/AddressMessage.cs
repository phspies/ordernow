
using customer_microservice.Datamodels;

namespace customer_microservice.Kafka
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