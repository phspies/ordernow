using System;
using System.ComponentModel.DataAnnotations;

namespace order_microservice.Datamodels
{
    public class AddressDataModel
    {
        public Guid Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}