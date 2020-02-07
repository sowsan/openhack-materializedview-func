using System;
using System.Collections.Generic;
using System.Text;

namespace MaterializedViewFunction
{
    public class Detail
    {
        public string Email { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }

    public class MoviewOrder
    {
        public string Region { get; set; }
        public DateTime OrderDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public int SMSOptIn { get; set; }
        public string SMSStatus { get; set; }
        public string Email { get; set; }
        public string ReceiptUrl { get; set; }
        public double Total { get; set; }
        public string PaymentTransactionId { get; set; }
        public int HasBeenShipped { get; set; }
        public List<Detail> Details { get; set; }
        public DateTime EventProcessedUtcTime { get; set; }
        public int PartitionId { get; set; }
        public DateTime EventEnqueuedUtcTime { get; set; }
    }
}
