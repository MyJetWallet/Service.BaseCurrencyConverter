using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.AssetsDictionary.Domain.Models;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Grpc;
using Service.BaseCurrencyConverter.Grpc.Models;
using Service.BaseCurrencyConverter.Settings;

namespace Service.BaseCurrencyConverter.Services
{
    public class BaseCurrencyConverterService : IBaseCurrencyConverterService
    {
        private readonly ILogger<IBaseCurrencyConverterService> _logger;
        private readonly IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> _writer;

        public BaseCurrencyConverterService(
            ILogger<IBaseCurrencyConverterService> logger, 
            IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> writer)
        {
            _logger = logger;
            _writer = writer;
        }


        public async Task<BaseAssetConvertMap> GetConvertorMapToBaseCurrencyAsync(GetConvertorMapToBaseCurrencyRequest request)
        {
            var entity = await _writer.GetAsync(BaseAssetConvertMapNoSql.GeneratePartitionKey(request.BrokerId), BaseAssetConvertMapNoSql.GenerateRowKey(request.BaseAsset));

            var response = new BaseAssetConvertMap()
            {
                BaseAssetSymbol = request.BaseAsset,
                Maps = new List<BaseAssetConvertMapItem>()
            };

            if (entity == null)
            {
                _logger.LogError("Cannot find convert map for {symbol}, broker: {brokerId}", request.BaseAsset, request.BrokerId);
                return response;
            }

            response.Maps = entity.Map.Maps;

            return response;
        }
    }
}
