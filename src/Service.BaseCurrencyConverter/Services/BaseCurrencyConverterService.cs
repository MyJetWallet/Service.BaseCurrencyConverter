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
        private readonly IAssetsDictionaryClient _assetsDictionary;
        private readonly ISpotInstrumentDictionaryClient _instrumentDictionary;
        private readonly IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> _writer;
        private readonly List<string> _crossAssets;

        public BaseCurrencyConverterService(ILogger<IBaseCurrencyConverterService> logger, IAssetsDictionaryClient assetsDictionary, ISpotInstrumentDictionaryClient instrumentDictionary,
            SettingsModel settings, IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> writer)
        {
            _logger = logger;
            _assetsDictionary = assetsDictionary;
            _instrumentDictionary = instrumentDictionary;
            _writer = writer;
            _crossAssets = settings.CrossAssetsList.Split(";").ToList();
        }


        public async Task<BaseAssetConvertMap> GetConvertorMapToBaseCurrencyAsync(GetConvertorMapToBaseCurrencyRequest request)
        {
            var assets = _assetsDictionary.GetAssetsByBroker(new JetBrokerIdentity() {BrokerId = request.BrokerId});
            var instruments = _instrumentDictionary.GetSpotInstrumentByBroker(new JetBrokerIdentity() { BrokerId = request.BrokerId });

            var response = new BaseAssetConvertMap()
            {
                BrokerId = request.BrokerId,
                BaseAssetSymbol = request.BaseAsset,
                Maps = new List<BaseAssetConvertMapItem>()
            };

            foreach (var asset in assets)
            {
                if (asset.Symbol == request.BaseAsset)
                {
                    response.Maps.Add(new BaseAssetConvertMapItem()
                    {
                        AssetSymbol = asset.Symbol,
                        Operations = new List<BaseAssetConvertMapOperation>()
                    });

                    continue;
                }

                var instrument = instruments.FirstOrDefault(e => e.BaseAsset == request.BaseAsset && e.QuoteAsset == asset.Symbol && e.IsEnabled);
                
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
                                IsMultiply = false
                            }
                        }
                    });

                    continue;
                }

                instrument = instruments.FirstOrDefault(e => e.BaseAsset == asset.Symbol && e.QuoteAsset == request.BaseAsset && e.IsEnabled);

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
                                IsMultiply = true
                            }
                        }
                    });

                    continue;
                }

                var crossWay = FindCrossAssetWay(assets, instruments, asset.Symbol, request.BaseAsset);
                if (crossWay != null)
                {
                    response.Maps.Add(new BaseAssetConvertMapItem()
                    {
                        AssetSymbol = asset.Symbol,
                        Operations = crossWay
                    });

                    continue;
                }

                _logger.LogWarning("Cannot find way to convert {fromAssetSymbol} to {toAssetSymbol}", asset.Symbol, request.BaseAsset);
            }

            var entity = BaseAssetConvertMapNoSql.Create(response);

            await _writer.InsertOrReplaceAsync(entity);

            return response;
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
                                IsMultiply = instrument1.BaseAsset == fromAsset
                            },

                            new BaseAssetConvertMapOperation()
                            {
                                Order = 2,
                                InstrumentPrice = instrument2.Symbol,
                                IsMultiply = instrument2.BaseAsset == crossAsset.Symbol
                            },
                        };
                    }
                }
            }

            return null;
        }
}
}
