namespace EcloudvalleyHomework.DataTransferObjects
{
    public class UsageAmountDto
    {
        public string ProductName { get; set; } = null!;
        public Dictionary<string, decimal> DailyUsageAmounts { get; set; } = new();
    }

    public class UsageReportItem
    {
        public string ProductName { get; set; } = null!;
        public DateTime UsageStartDate { get; set; }
        public DateTime UsageEndDate { get; set; }
        public decimal DailyUsageAmount { get; set; }
    }
}
