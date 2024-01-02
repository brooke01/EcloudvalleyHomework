using System;
using System.Collections.Generic;

namespace EcloudvalleyHomework.Models
{
    public partial class UsageReport
    {
        public int UsageReportId { get; set; }
        public decimal PayerAccountId { get; set; }
        public decimal UnblendedCost { get; set; }
        public decimal UnblendedRate { get; set; }
        public decimal UsageAccountId { get; set; }
        public decimal UsageAmount { get; set; }
        public DateTime UsageStartDate { get; set; }
        public DateTime UsageEndDate { get; set; }
        public string ProductName { get; set; } = null!;
    }
}
