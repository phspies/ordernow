using System;
using System.ComponentModel.DataAnnotations;

namespace order_microservice.Datamodels
{
    public class OrderDataModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SupplierId { get; set; }
        public int StatusCode { get; set; }
        public DateTime OrderPlaced { get; set; }
        public DateTime OrderFilled { get; set; }
        public string Memo { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
    public class CreateOrderDataModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SupplierId { get; set; }
        public string Memo { get; set; }
    }
    public class UpdateOrderDataModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SupplierId { get; set; }
        public string Memo { get; set; }
    }

}
