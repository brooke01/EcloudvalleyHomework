using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using EcloudvalleyHomework.DataTransferObjects;

namespace EcloudvalleyHomework.CsvHelperMap
{
    public class CsvDtoMap : ClassMap<CsvDto>
    {
        public CsvDtoMap()
        {
            Map(m => m.PayerAccountId).Name("bill/PayerAccountId").TypeConverter<StringToDecimalConverter>();
            Map(m => m.UnblendedCost).Name("lineItem/UnblendedCost").TypeConverter<StringToDecimalConverter>();
            Map(m => m.UnblendedRate).Name("lineItem/UnblendedRate").TypeConverter<StringToDecimalConverter>();
            Map(m => m.UsageAccountId).Name("lineItem/UsageAccountId").TypeConverter<StringToDecimalConverter>();
            Map(m => m.UsageAmount).Name("lineItem/UsageAmount").TypeConverter<StringToDecimalConverter>();
            Map(m => m.UsageStartDate).Name("lineItem/UsageStartDate");
            Map(m => m.UsageEndDate).Name("lineItem/UsageEndDate");
            Map(m => m.ProductName).Name("product/ProductName");
        }
    }

    public class StringToDecimalConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text)) return 0M;
                return Convert.ToDecimal(Convert.ToDouble(text));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
