using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace customer_microservice.Datamodels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AddressKafkaMessage
    {
        [JsonProperty("action")]
        public ActionEnum Action { get; set; }
        [JsonProperty("id")]
        public Guid AddressID { get; set; }
        [JsonProperty("address")]
        public AddressDataModel Address { get; set; }
    }
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
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
    }
    public class UpdateAddressDataModel 
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
        [JsonProperty("row_version")]
        public byte[] RowVersion { get; set; }
    }
}
