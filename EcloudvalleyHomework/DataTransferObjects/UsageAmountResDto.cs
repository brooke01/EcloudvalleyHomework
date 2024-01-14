namespace EcloudvalleyHomework.DataTransferObjects
{
    public class UsageAmountResDto
    {
        public string ProductName { get; set; } = null!;
        public IEnumerable<UsageAmountDetailDto> UsageAmountDetail { get; set; } = Enumerable.Empty<UsageAmountDetailDto>();
    }
}
