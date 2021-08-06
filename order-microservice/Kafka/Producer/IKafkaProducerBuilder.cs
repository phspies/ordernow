using Confluent.Kafka;
using order_microservice.Datamodels;

namespace order_microservice.Kafka
{
    public interface IKafkaProducerBuilder
    {
        IProducer<string, string> Build();
    }
}