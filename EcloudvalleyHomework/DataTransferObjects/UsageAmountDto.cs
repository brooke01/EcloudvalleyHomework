namespace EcloudvalleyHomework.DataTransferObjects
{
    public class UsageAmountDto
    {
        public string ProductName { get; set; } = null!;
        public Dictionary<string, decimal> DailyUsageAmounts { get; set; } = new();
    }
}
