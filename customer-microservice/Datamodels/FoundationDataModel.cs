using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace customer_microservice.Datamodels
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class FoundationDataModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("create_timestamp")]
        public DateTime CreateTimeStamp { get; set; }
        [JsonProperty("update_timestamp")]
        public DateTime UpdateTimeStamp { get; set; }
        [JsonProperty("row_version")]
        public byte[] RowVersion { get; set; }
    }

    public enum ActionEnum
    {
        create, update, delete
    }
}
