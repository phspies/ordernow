using System;
using Confluent.Kafka;
using customer_microservice.Datamodels;
using Microsoft.Extensions.Options;

namespace customer_microservice.Kafka
{
    public class KafkaProducerBuilder : IKafkaProducerBuilder
    {
        private readonly KafkaOptions _kafkaOptions;

        public KafkaProducerBuilder(IOptions<KafkaOptions> producerWorkerOptions)
        {
            _kafkaOptions = producerWorkerOptions?.Value ?? throw new ArgumentNullException(nameof(producerWorkerOptions));
        }

        public IProducer<string, string> Build()
        {
            var config = new ClientConfig
            {
                BootstrapServers = _kafkaOptions.KafkaBootstrapServers
            };

            var producerBuilder = new ProducerBuilder<string, string>(config);

            return producerBuilder.Build();
        }
    }
}