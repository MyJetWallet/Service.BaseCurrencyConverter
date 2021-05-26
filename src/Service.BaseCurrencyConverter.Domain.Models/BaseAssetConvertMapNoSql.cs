using MyNoSqlServer.Abstractions;

namespace Service.BaseCurrencyConverter.Domain.Models
{
    public class BaseAssetConvertMapNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-basecurrency-convertmap";

        public static string GeneratePartitionKey(string brokerId) => brokerId;

        public static string GenerateRowKey(string baseAsset) => baseAsset;

        public BaseAssetConvertMap Map { get; set; }

        public static BaseAssetConvertMapNoSql Create(BaseAssetConvertMap map, string brokerId)
        {
            return new BaseAssetConvertMapNoSql()
            {
                PartitionKey = GeneratePartitionKey(brokerId),
                RowKey = GenerateRowKey(map.BaseAssetSymbol),
                Map = map
            };
        }
    }
}