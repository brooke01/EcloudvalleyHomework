namespace EcloudvalleyHomework.DataTransferObjects
{
    public class UsageReportItemWithDateDto
    {
        public string ProductName { get; set; } = null!;
        public string Date { get; set; } = "";
        public decimal DailyUsageAmount { get; set; }
    }
}
