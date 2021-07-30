using System.Threading;
using System.Threading.Tasks;

namespace customer_microservice.Kafka
{
    public interface IMessageProducer
    {
        Task ProduceAsync(string key, IMessage message, CancellationToken cancellationToken);
    }
}