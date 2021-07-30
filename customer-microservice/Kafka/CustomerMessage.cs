
using customer_microservice.Datamodels;
using Newtonsoft.Json;

namespace customer_microservice.Kafka
{
    [MessageTopic("ordernow-address-events")]
    public class AddressMessage : IMessage
    {
        public AddressMessage(AddressKafkaMessage _addressObject)
        {
            addressObject = _addressObject;
        }
        [JsonProperty("address_kafka_message")]
        public AddressKafkaMessage addressObject { get; }
    }
}