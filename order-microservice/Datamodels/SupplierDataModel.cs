using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using order_microservice.Kafka;

namespace order_microservice.Datamodels
{
    [JsonObject(MemberSerialization.OptIn, Title = "SupplierKafkaMessage")]
    [MessageTopic("ordernow-supplier-events")]
    public class SupplierKafkaMessage : IMessage
    {
        [JsonProperty("action")]
        public ActionEnum Action { get; set; }
        [JsonProperty("id")]
        public Guid SupplierID { get; set; }
        [JsonProperty("supplier_data_model")]
        public SupplierDataModel Supplier { get; set; }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class SupplierDataModel : FoundationDataModel
    {
        [JsonRequired]
        [JsonProperty("supplier_name")]
        public string SupplierName { get; set; }
        [JsonProperty("telephone_number")]
        public string TelephoneNumber { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        [JsonProperty("website_url")]
        public string WebsiteURL { get; set; }
        [JsonProperty("address")]
        public AddressDataModel Address { get; set; }
        [JsonProperty("supplier_status")]
        public SupplierStatusEnum SupplierStatus { get; set; }
    }
    public class UpdateSupplierDataModel : FoundationUpdateDataModel
    {
        [JsonRequired]
        [JsonProperty("supplier_name")]
        public string SupplierName { get; set; }
        [JsonProperty("telephone_number")]
        public string TelephoneNumber { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        [JsonProperty("website_url")]
        public string WebsiteURL { get; set; }
        [JsonProperty("address")]
        public AddressDataModel Address { get; set; }
        [JsonProperty("supplier_status")]
        public SupplierStatusEnum SupplierStatus { get; set; }
    }
    public class CreateSupplierDataModel
    {
        [JsonRequired]
        [JsonProperty("supplier_name")]
        public string SupplierName { get; set; }
        [JsonProperty("telephone_number")]
        public string TelephoneNumber { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        [JsonProperty("website_url")]
        public string WebsiteURL { get; set; }
        [JsonProperty("address")]
        public AddressDataModel Address { get; set; }
        [JsonProperty("supplier_status")]
        public SupplierStatusEnum SupplierStatus { get; set; }
    }

    public enum SupplierStatusEnum
    {
        active, suspended, inactive
    }
}
