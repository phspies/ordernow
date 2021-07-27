using System;
using System.ComponentModel.DataAnnotations;

namespace order_microservice.Datamodels
{
    public abstract class FoundationDataModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTimeStamp { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

}
