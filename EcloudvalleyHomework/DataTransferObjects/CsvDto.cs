namespace EcloudvalleyHomework.DataTransferObjects
{
    public class CsvDto
    {
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
