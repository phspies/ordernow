using System;
using System.ComponentModel.DataAnnotations;

namespace customer_microservice.Datamodels
{
    public class CustomerDataModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
        public double? CurrentAccountValue { get; set; }
        public double? TotalBuyValue { get; set; }
        public double? CurrentCreditValue { get; set; }

        public DateTime CreateTimeStamp { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
    public class UpdateCustomerDataModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
    public class CreateCustomerDataModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
    }
}
