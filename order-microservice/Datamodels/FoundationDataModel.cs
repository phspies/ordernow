using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace order_microservice.Datamodels
{
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
    public abstract class FoundationUpdateDataModel
    {
        [JsonProperty("row_version")]
        public byte[] RowVersion { get; set; }
    }
    public enum ActionEnum
    {
        create, update, delete
    }

}
