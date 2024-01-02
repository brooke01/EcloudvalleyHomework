namespace EcloudvalleyHomework.DataTransferObjects
{
    public class UsageReportItemDto
    {
        public string ProductName { get; set; } = null!;
        public DateTime UsageStartDate { get; set; }
        public DateTime UsageEndDate { get; set; }
        public decimal DailyUsageAmount { get; set; }
    }
}
