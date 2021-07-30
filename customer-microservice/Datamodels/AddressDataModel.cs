using System;
using System.ComponentModel.DataAnnotations;
using customer_microservice.Kafka;
using Newtonsoft.Json;

namespace customer_microservice.Datamodels
{
    [JsonObject(MemberSerialization.OptIn, Title = "AddressKafkaMessage")]
    [MessageTopic("ordernow-address-events")]
    public class AddressKafkaMessage : IMessage
    {
        [JsonProperty("action")]
        public ActionEnum Action { get; set; }
        [JsonProperty("id")]
        public Guid AddressID { get; set; }
        [JsonProperty("address_data_model")]
        public AddressDataModel Address { get; set; }
    }
    [JsonObject(MemberSerialization.OptIn, Title = "AddressDataModel")]
    public class AddressDataModel : FoundationDataModel
    {
        [JsonProperty("address_1")]
        public string Address1 { get; set; }
        [JsonProperty("address_2")]
        public string Address2 { get; set; }
        [JsonProperty("address_3")]
        public string Address3 { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("zipcode")]
        public int? ZipCode { get; set; }
    }
    public class CreateAddressDataModel
    {
        [JsonProperty("address_1")]
        public string Address1 { get; set; }
        [JsonProperty("address_2")]
        public string Address2 { get; set; }
        [JsonProperty("address_3")]
        public string Address3 { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("zipcode")]
        public int? ZipCode { get; set; }
    }
    public class UpdateAddressDataModel 
    {
        [JsonProperty("address_1")]
        public string Address1 { get; set; }
        [JsonProperty("address_2")]
        public string Address2 { get; set; }
        [JsonProperty("address_3")]
        public string Address3 { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("zipcode")]
        public int? ZipCode { get; set; }
        [JsonProperty("row_version")]
        public byte[] RowVersion { get; set; }
    }
}
