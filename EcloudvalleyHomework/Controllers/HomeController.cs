using CsvHelper;
using CsvHelper.Configuration;
using EcloudvalleyHomework.CsvHelperMap;
using EcloudvalleyHomework.DataTransferObjects;
using EcloudvalleyHomework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace EcloudvalleyHomework.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AwsDbContext _context;
        public HomeController(
            IConfiguration configuration,
            AwsDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [RequestSizeLimit(500 * 1024 * 1024)]
        [HttpPost("Data")]
        public async Task<IActionResult> Data(IFormFile file)
        {
            try
            {
                if (ValidateFileExtention(file.FileName) == false) return BadRequest("僅允許上傳 .csv 檔案");

                DataTable dtCsvData = new DataTable();
                dtCsvData.Columns.Add("usage_report_id", typeof(int));
                dtCsvData.Columns.Add("payer_account_id", typeof(decimal));
                dtCsvData.Columns.Add("unblended_cost", typeof(decimal));
                dtCsvData.Columns.Add("unblended_rate", typeof(decimal));
                dtCsvData.Columns.Add("usage_account_id", typeof(decimal));
                dtCsvData.Columns.Add("usage_amount", typeof(decimal));
                dtCsvData.Columns.Add("usage_start_date", typeof(DateTime));
                dtCsvData.Columns.Add("usage_end_date", typeof(DateTime));
                dtCsvData.Columns.Add("product_name", typeof(string));

                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.Context.RegisterClassMap<CsvDtoMap>();
                    var records = csv.GetRecords<CsvDto>().ToList();

                    records.ForEach(x =>
                    {
                        DataRow dr = dtCsvData.NewRow();
                        dr["payer_account_id"] = x.PayerAccountId;
                        dr["unblended_cost"] = x.UnblendedCost;
                        dr["unblended_rate"] = x.UnblendedRate;
                        dr["usage_account_id"] = x.UsageAccountId;
                        dr["usage_amount"] = x.UsageAmount;
                        dr["usage_start_date"] = x.UsageStartDate;
                        dr["usage_end_date"] = x.UsageEndDate;
                        dr["product_name"] = x.ProductName;
                        dtCsvData.Rows.Add(dr);
                    });
                }

                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "usage_report";
                        await bulkCopy.WriteToServerAsync(dtCsvData);
                    }
                }

                return CreatedAtAction("Data", new { }, new { Message = $"import success, total is {dtCsvData.Rows.Count}" });
            }
            catch (Exception ex)
            {
                return Problem($"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("UnblendedCost/{usageAccountId}")]
        public async Task<IActionResult> UnblendedCost(decimal usageAccountId)
        {
            var data = _context.UsageReports
                .Where(x => x.UsageAccountId == usageAccountId)
                .GroupBy(x => x.ProductName)
                .Select(x => new UnblendedCostDto { ProductName = x.Key, Sum = x.Sum(x => x.UnblendedCost) });

            return Ok(await data.ToDictionaryAsync(x => x.ProductName, x => x.Sum));
        }

        [HttpGet("UsageAmount/{usageAccountId}")]
        public async Task<IActionResult> UsageAmount(decimal usageAccountId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (startDate == null && endDate != null) startDate = endDate.Value.AddDays(-30);
                if (endDate == null && startDate != null) endDate = startDate.Value.AddDays(30);
                if (startDate == null && endDate == null)
                {
                    endDate = DateTime.UtcNow;
                    startDate = endDate.Value.AddDays(-30);
                }
                if (startDate > endDate) return BadRequest("查詢條件的起迄日範圍有誤");
                if (startDate < endDate.Value.AddDays(-30)) return BadRequest("查詢範圍限定在30天內");

                var originData = _context.UsageReports
                    .Where(u => u.UsageAccountId == usageAccountId &&
                        (u.UsageStartDate.Date >= startDate.Value.Date && u.UsageStartDate.Date <= endDate.Value.Date
                        || u.UsageEndDate.Date >= startDate.Value.Date && u.UsageEndDate.Date <= endDate.Value.Date)
                    )
                    .Select(x => new UsageReportItemDto
                    {
                        ProductName = x.ProductName,
                        UsageStartDate = x.UsageStartDate,
                        UsageEndDate = x.UsageEndDate,
                        DailyUsageAmount = Math.Round(x.UsageAmount / ((x.UsageEndDate.Date - x.UsageStartDate.Date).Days + 1), 9)
                    });

                var productNames = originData.GroupBy(x => x.ProductName).Select(x => x.Key);
                List<UsageAmountDto> result = new List<UsageAmountDto>();
                foreach (var productName in productNames)
                {
                    Dictionary<string, decimal> dailyUsageAmounts = new();
                    for (int i = 0; i <= (endDate.Value.Date - startDate.Value.Date).Days; i++)
                    {
                        var day = startDate.Value.AddDays(i);
                        dailyUsageAmounts.Add(day.ToString("yyyy/MM/dd"), originData.AsEnumerable().Where(x => x.ProductName == productName
                                                                                 && x.UsageStartDate.Date <= day.Date
                                                                                 && x.UsageEndDate.Date >= day.Date)
                                                                                 .Sum(x => x.DailyUsageAmount)
                        );
                    }
                    result.Add(new UsageAmountDto { ProductName = productName, DailyUsageAmounts = dailyUsageAmounts });
                }

                return Ok(result.ToDictionary(x => x.ProductName, x => x.DailyUsageAmounts));
            }
            catch (Exception ex)
            {
                return Problem($"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("UsageAmount_V2/{usageAccountId}")]
        public async Task<IActionResult> UsageAmount(decimal usageAccountId, DateTime? queryStartDate, int pageIndex = 1, int pageSize = 10)
        {
            const int PRECISION = 9;
            try
            {
                queryStartDate = queryStartDate ?? (DateTime?)System.Data.SqlTypes.SqlDateTime.MinValue;

                var originData = _context.UsageReports
                    .Where(x => x.UsageAccountId == usageAccountId && x.UsageStartDate >= queryStartDate.Value.Date)
                    .Select(x => new UsageReportItemDto
                    {
                        ProductName = x.ProductName,
                        UsageStartDate = x.UsageStartDate,
                        UsageEndDate = x.UsageEndDate,
                        DailyUsageAmount = Math.Round(x.UsageAmount / ((x.UsageEndDate.Date - x.UsageStartDate.Date).Days + 1), PRECISION)
                    });

                List<UsageAmountDto> result = new List<UsageAmountDto>();
                await originData.GroupBy(x => x.ProductName).Select(x => x.Key).ForEachAsync(productName =>
                {
                    Dictionary<string, decimal> dailyUsageAmounts = new();
                    for (int i = 0; i < pageSize; i++)
                    {
                        var day = queryStartDate.Value.AddDays(i + (pageIndex - 1) * pageSize);
                        var amount = originData.AsEnumerable()
                                               .Where(x => x.ProductName == productName && x.UsageStartDate.Date <= day.Date && day.Date <= x.UsageEndDate.Date)
                                               .Sum(x => x.DailyUsageAmount);
                        dailyUsageAmounts.Add(day.ToString("yyyy/MM/dd"), amount);
                    }
                    result.Add(new UsageAmountDto { ProductName = productName, DailyUsageAmounts = dailyUsageAmounts });
                });

                return Ok(result.ToDictionary(x => x.ProductName, x => x.DailyUsageAmounts));
            }
            catch (Exception ex)
            {
                return Problem($"Internal server error: {ex.Message}");
            }
        }

        private bool ValidateFileExtention(string fileName)
        {
            var allowedExtensions = new[] { ".csv" }; // 允許的檔案擴展名
            var fileExtension = Path.GetExtension(fileName).ToLower();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
