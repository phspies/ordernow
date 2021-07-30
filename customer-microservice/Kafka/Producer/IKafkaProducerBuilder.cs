using Confluent.Kafka;
using customer_microservice.Datamodels;

namespace customer_microservice.Kafka
{
    public interface IKafkaProducerBuilder
    {
        IProducer<string, string> Build();
    }
}