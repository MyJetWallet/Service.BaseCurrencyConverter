using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Grpc.Models;

namespace Service.BaseCurrencyConverter.Services
{
    public class NoSqlCleanJob: IStartable, IDisposable
    {
        private readonly IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> _dataWriter;
        private readonly ILogger<NoSqlCleanJob> _logger;
        private readonly BaseCurrencyConverterService _baseCurrencyConverterService;
        private readonly IAssetsDictionaryClient _assetsDictionary;

        private MyTaskTimer _timer;

        public NoSqlCleanJob(IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> dataWriter, IAssetsDictionaryClient assetsDictionary, ISpotInstrumentDictionaryClient instrumentDictionary, ILogger<NoSqlCleanJob> logger,
            BaseCurrencyConverterService baseCurrencyConverterService)
        {
            _dataWriter = dataWriter;
            _logger = logger;
            _baseCurrencyConverterService = baseCurrencyConverterService;
            _assetsDictionary = assetsDictionary;
            assetsDictionary.OnChanged += AssetsAndInstrumentDictionary_OnChanged;
            instrumentDictionary.OnChanged += AssetsAndInstrumentDictionary_OnChanged;

            _timer = new MyTaskTimer(nameof(NoSqlCleanJob), TimeSpan.FromMinutes(5), logger, DoTime);
        }

        private async Task DoTime()
        {
            foreach (var asset in _assetsDictionary.GetAllAssets().Where(e => e.IsEnabled))
            {
                await _baseCurrencyConverterService.GetConvertorMapToBaseCurrencyAsync(
                    new GetConvertorMapToBaseCurrencyRequest()
                    {
                        BrokerId = asset.BrokerId,
                        BaseAsset = asset.Symbol
                    });
            }

            _logger.LogInformation("Nosql table is recalculated");
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
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
