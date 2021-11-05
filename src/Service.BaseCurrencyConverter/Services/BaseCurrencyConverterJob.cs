using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.AssetsDictionary.Domain.Models;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Grpc;
using Service.BaseCurrencyConverter.Settings;

namespace Service.BaseCurrencyConverter.Services
{
    public class BaseCurrencyConverterJob: IDisposable
    {
        private readonly ILogger<IBaseCurrencyConverterService> _logger;
        private readonly IAssetsDictionaryClient _assetsDictionary;
        private readonly ISpotInstrumentDictionaryClient _instrumentDictionary;
        private readonly IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> _writer;
        private readonly List<string> _crossAssets;

        private readonly MyTaskTimer _timer;

        public BaseCurrencyConverterJob(
            ILogger<IBaseCurrencyConverterService> logger, 
            IAssetsDictionaryClient assetsDictionary, 
            ISpotInstrumentDictionaryClient instrumentDictionary,
            SettingsModel settings, 
            IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> writer)
        {
            _logger = logger;
            _assetsDictionary = assetsDictionary;
            _instrumentDictionary = instrumentDictionary;
            _writer = writer;
            _crossAssets = settings.CrossAssetsList.Split(";").ToList();
            _timer = new MyTaskTimer(nameof(BaseCurrencyConverterJob), TimeSpan.FromMinutes(5), logger, DoTime);
        }

        public void Start()
        {
            _timer.Start();
        }

        private async Task DoTime()
        {
            _logger.LogInformation("Start recalculation of convert maps...");

            var assets = _assetsDictionary.GetAllAssets();

            foreach (var asset in assets.Where(e => e.IsEnabled))
            {
                await GetConvertorMapToBaseCurrencyAsync(asset.BrokerId, asset);
                _logger.LogInformation("Recalculation done for: {symbol}", asset.Symbol);
            }

            _logger.LogInformation("Recalculation of convert maps is done");
        }


        public async Task GetConvertorMapToBaseCurrencyAsync(string brokerId, IAsset baseAsset)
        {
            var assets = _assetsDictionary.GetAssetsByBroker(new JetBrokerIdentity() {BrokerId = brokerId});
            var instruments = _instrumentDictionary.GetSpotInstrumentByBroker(new JetBrokerIdentity() { BrokerId = brokerId });

            var response = new BaseAssetConvertMap()
            {
                BaseAssetSymbol = baseAsset.Symbol,
                Maps = new List<BaseAssetConvertMapItem>()
            };

            foreach (var asset in assets.Where(e => e.IsEnabled))
            {
                if (baseAsset.IsMainNet != asset.IsMainNet)
                    continue;

                if (asset.Symbol == baseAsset.Symbol)
                {
                    response.Maps.Add(new BaseAssetConvertMapItem()
                    {
                        AssetSymbol = asset.Symbol,
                        Operations = new List<BaseAssetConvertMapOperation>()
                    });

                    continue;
                }

                var instrument = instruments.FirstOrDefault(e => e.BaseAsset == baseAsset.Symbol && e.QuoteAsset == asset.Symbol && e.IsEnabled);
                
                if (instrument != null)
                {
                    response.Maps.Add(new BaseAssetConvertMapItem()
                    {
                        AssetSymbol = asset.Symbol,
                        Operations = new List<BaseAssetConvertMapOperation>()
                        {
                            new BaseAssetConvertMapOperation()
                            {
                                Order = 1,
                                InstrumentPrice = instrument.Symbol,
                                IsMultiply = false,
                                UseBid = true
                            }
                        }
                    });

                    continue;
                }

                instrument = instruments.FirstOrDefault(e => e.BaseAsset == asset.Symbol && e.QuoteAsset == baseAsset.Symbol && e.IsEnabled);

                if (instrument != null)
                {
                    response.Maps.Add(new BaseAssetConvertMapItem()
                    {
                        AssetSymbol = asset.Symbol,
                        Operations = new List<BaseAssetConvertMapOperation>()
                        {
                            new BaseAssetConvertMapOperation()
                            {
                                Order = 1,
                                InstrumentPrice = instrument.Symbol,
                                IsMultiply = true,
                                UseBid = true
                            }
                        }
                    });

                    continue;
                }

                var crossWay = FindCrossAssetWay(assets, instruments, asset.Symbol, baseAsset.Symbol);
                if (crossWay != null)
                {
                    response.Maps.Add(new BaseAssetConvertMapItem()
                    {
                        AssetSymbol = asset.Symbol,
                        Operations = crossWay
                    });

                    continue;
                }

                _logger.LogError("Cannot find way to convert {fromAssetSymbol} to {toAssetSymbol}", asset.Symbol, baseAsset.Symbol);
            }

            var entity = BaseAssetConvertMapNoSql.Create(response, brokerId);
            
            await _writer.InsertOrReplaceAsync(entity);
        }

        private List<BaseAssetConvertMapOperation> FindCrossAssetWay(IReadOnlyList<IAsset> assets, IReadOnlyList<ISpotInstrument> instruments, string fromAsset, string toAsset)
        {
            foreach (var crossAssetId in _crossAssets)
            {
                var crossAsset = assets.FirstOrDefault(e => e.Symbol == crossAssetId);
                if (crossAsset != null)
                {
                    var instrument1 = instruments
                        .Where(e => e.IsEnabled)
                        .FirstOrDefault(e =>
                            (e.BaseAsset == fromAsset && e.QuoteAsset == crossAsset.Symbol) ||
                            (e.BaseAsset == crossAsset.Symbol && e.QuoteAsset == fromAsset));

                    var instrument2 = instruments
                        .Where(e => e.IsEnabled)
                        .FirstOrDefault(e =>
                            (e.BaseAsset == crossAsset.Symbol && e.QuoteAsset == toAsset) ||
                            (e.BaseAsset == toAsset && e.QuoteAsset == crossAsset.Symbol));

                    if (instrument1 != null && instrument2 != null)
                    {
                        return new List<BaseAssetConvertMapOperation>()
                        {
                            new BaseAssetConvertMapOperation()
                            {
                                Order = 1,
                                InstrumentPrice = instrument1.Symbol,
                                IsMultiply = instrument1.BaseAsset == fromAsset,
                                UseBid = true
                            },

                            new BaseAssetConvertMapOperation()
                            {
                                Order = 2,
                                InstrumentPrice = instrument2.Symbol,
                                IsMultiply = instrument2.BaseAsset == crossAsset.Symbol,
                                UseBid = true
                            },
                        };
                    }
                }
            }

            return null;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
