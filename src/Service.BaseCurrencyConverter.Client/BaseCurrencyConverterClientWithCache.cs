using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Grpc;
using Service.BaseCurrencyConverter.Grpc.Models;

namespace Service.BaseCurrencyConverter.Client
{
    public class BaseCurrencyConverterClientWithCache : IBaseCurrencyConverterService
    {
        private readonly IBaseCurrencyConverterService _client;
        private readonly IMyNoSqlServerDataReader<BaseAssetConvertMapNoSql> _reader;

        public BaseCurrencyConverterClientWithCache(IBaseCurrencyConverterService client, IMyNoSqlServerDataReader<BaseAssetConvertMapNoSql> reader)
        {
            _client = client;
            _reader = reader;
        }

        public Task<BaseAssetConvertMap> GetConvertorMapToBaseCurrencyAsync(GetConvertorMapToBaseCurrencyRequest request)
        {
            var entity = _reader.Get(BaseAssetConvertMapNoSql.GeneratePartitionKey(request.BrokerId), BaseAssetConvertMapNoSql.GenerateRowKey(request.BaseAsset));
            if (entity?.Map != null)
                return Task.FromResult(entity.Map);

            return _client.GetConvertorMapToBaseCurrencyAsync(request);
        }
    }
}