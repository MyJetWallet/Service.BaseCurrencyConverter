using System;
using Autofac;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;

namespace Service.BaseCurrencyConverter.Services
{
    public class NoSqlCleanJob: IStartable
    {
        private readonly IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> _dataWriter;
        private readonly ILogger<NoSqlCleanJob> _logger;

        public NoSqlCleanJob(IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> dataWriter, IAssetsDictionaryClient assetsDictionary, ISpotInstrumentDictionaryClient instrumentDictionary, ILogger<NoSqlCleanJob> logger)
        {
            _dataWriter = dataWriter;
            _logger = logger;
            assetsDictionary.OnChanged += AssetsAndInstrumentDictionary_OnChanged;
            instrumentDictionary.OnChanged += AssetsAndInstrumentDictionary_OnChanged;
        }

        private void AssetsAndInstrumentDictionary_OnChanged()
        {
            try
            {
                _dataWriter.CleanAndKeepMaxPartitions(0).GetAwaiter().GetResult();
                _logger.LogInformation("Nosql table is clear");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot cleanup nosql table");
            }
        }

        public void Start()
        {
            AssetsAndInstrumentDictionary_OnChanged();
        }
    }
}
