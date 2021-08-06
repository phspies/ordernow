using Microsoft.Extensions.DependencyInjection;

namespace order_microservice.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaProducer(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaProducerBuilder, KafkaProducerBuilder>();

            services.AddSingleton<IMessageProducer, KafkaMessageProducer>();

            return services;
        }
    }
}