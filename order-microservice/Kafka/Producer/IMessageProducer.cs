using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace order_microservice.Kafka
{
    public interface IMessageProducer
    {
        Task<string> ProduceAsync(string key, IMessage message, CancellationToken cancellationToken);
    }
}