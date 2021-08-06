namespace order_microservice.Kafka
{
    public class KafkaOptions
    {
        public string KafkaBootstrapServers { get; set; }
        public string ConsumerGroupId { get; set; }
        public bool EnableIdempotence { get; set; }
    }
}