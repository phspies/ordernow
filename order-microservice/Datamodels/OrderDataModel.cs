using System;
using Newtonsoft.Json;
using order_microservice.Kafka;

namespace order_microservice.Datamodels
{
    [JsonObject(MemberSerialization.OptIn, Title = "OrderrKafkaMessage")]
    [MessageTopic("ordernow-supplier-events")]
    public class OrderKafkaMessage : IMessage
    {
        [JsonProperty("action")]
        public ActionEnum Action { get; set; }
        [JsonProperty("id")]
        public Guid SupplierID { get; set; }
        [JsonProperty("supplier_data_model")]
        public SupplierDataModel Supplier { get; set; }
    }
    public class OrderDataModel : FoundationDataModel
    {
        [JsonProperty("customer")]
        public CustomerDataModel Customer { get; set; }
        [JsonProperty("supplier")]
        public OrderDataModel Supplier { get; set; }
        [JsonProperty("status_code")]
        public OrderStatusEnum StatusCode { get; set; }
        [JsonProperty("datetime_order_placed")]
        public DateTime OrderPlaced { get; set; }
        [JsonProperty("datetime_order_filled")]
        public DateTime OrderFilled { get; set; }
        [JsonProperty("memo")]
        public string Memo { get; set; }

    }
    public class CreateOrderDataModel
    {
        [JsonProperty("status_code")]
        public OrderStatusEnum StatusCode { get; set; }
        [JsonProperty("customer_id")]
        public string CustomerID { get; set; }
        [JsonProperty("supplier_id")]
        public string SupplierID { get; set; }
        [JsonProperty("memo")]
        public string Memo { get; set; }
    }
    public class UpdateOrderDataModel
    {
        [JsonProperty("status_code")]
        public OrderStatusEnum StatusCode { get; set; }
        [JsonProperty("memo")]
        public string Memo { get; set; }
    }

    public enum OrderStatusEnum
    {
        evaluating, pending, placed, cancelled
    }

}
