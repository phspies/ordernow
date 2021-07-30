using System;
using System.ComponentModel.DataAnnotations;
using customer_microservice.Kafka;
using Newtonsoft.Json;

namespace customer_microservice.Datamodels
{
    [JsonObject(MemberSerialization.OptIn, Title = "CustomerKafkaMessage")]
    [MessageTopic("ordernow-customer-events")]
    public class CustomerKafkaMessage : IMessage
    {
        [JsonProperty("action")]
        public ActionEnum Action { get; set; }
        [JsonProperty("id")]
        public Guid CustomerID { get; set; }
        [JsonProperty("customer_data_model")]
        public CustomerDataModel Customer { get; set; }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class CustomerDataModel
    {
        [JsonRequired]
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("address")]
        public AddressDataModel Address { get; set; }
        [JsonProperty("current_account_value")]
        public double? CurrentAccountValue { get; set; }
        [JsonProperty("total_buy_value")]
        public double? TotalBuyValue { get; set; }
        [JsonProperty("current_credit_value")]
        public double? CurrentCreditValue { get; set; }
        [JsonProperty("create_time_stamp")]
        public DateTime CreateTimeStamp { get; set; }
        [JsonProperty("update_time_stamp")]
        public DateTime UpdateTimeStamp { get; set; }
        [Timestamp]
        [JsonProperty("row_version")]
        public byte[] RowVersion { get; set; }
    }
    public class UpdateCustomerDataModel
    {
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [Timestamp]
        [JsonProperty("row_version")]
        public byte[] RowVersion { get; set; }
    }
    public class CreateCustomerDataModel
    {
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("address")]
        public CreateAddressDataModel Address { get; set; }
    }
}
