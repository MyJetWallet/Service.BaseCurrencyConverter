using System;
using System.Linq;
using Elasticsearch.Net.Specification.IndicesApi;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Service.AssetsDictionary.Domain.Models;
using Service.BaseCurrencyConverter.Grpc.Models;
using Service.BaseCurrencyConverter.Services;
using Service.BaseCurrencyConverter.Settings;

namespace Service.BaseCurrencyConverter.Tests
{
    public class TestExample
    {
        private AssetsDictionaryClientMock _assets;
        private SpotInstrumentDictionaryClientMock _instruments;
        private MyNoSqlServerDataWriterBaseAssetConvertMapNoSqlMock _writer = new MyNoSqlServerDataWriterBaseAssetConvertMapNoSqlMock();
        private SettingsModel _settings = new SettingsModel();

        private BaseCurrencyConverterService _service;

        public const string Broker = "jetwallet";

        [SetUp]
        public void Setup()
        {
            var loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    }));

            _settings.CrossAssetsList = "USD;BTC;ETH";

            _assets = new AssetsDictionaryClientMock();
            _instruments = new SpotInstrumentDictionaryClientMock();

            _service = new BaseCurrencyConverterService(loggerFactory.CreateLogger<BaseCurrencyConverterService>(), _assets, _instruments, _settings, _writer);
        }

        [Test]
        public void Test1()
        {
            AddAsset("ALGO");
            AddAsset("BTC");
            AddAsset("EUR");

            AddInstrument("ALGOUSD", "ALGO", "USD");
            AddInstrument("ALGOBTC", "ALGO", "BTC");
            AddInstrument("BTCEUR", "BTC", "EUR");
            AddInstrument("BTCUSD", "BTC", "USD");

            var result = _service.GetConvertorMapToBaseCurrencyAsync(new GetConvertorMapToBaseCurrencyRequest()
            {
                BaseAsset = "EUR",
                BrokerId = Broker
            }).GetAwaiter().GetResult();

            foreach (var map in result.Maps)
            {
                Console.Write($"{map.AssetSymbol} to EUR: ");
                foreach (var item in map.Operations.OrderBy(e => e.Order))
                {
                    Console.Write($"{item.InstrumentPrice}({(item.IsMultiply ? "*" : "/")}) ; ");   
                }

                Console.WriteLine();
            }
        }

        [Test]
        public void Test2()
        {
            AddAsset("ALGO");
            AddAsset("BTC");
            AddAsset("EUR");

            AddInstrument("ALGOUSD", "ALGO", "USD");
            AddInstrument("ALGOBTC", "ALGO", "BTC");
            AddInstrument("BTCEUR", "BTC", "EUR");
            AddInstrument("BTCUSD", "BTC", "USD");

            var result = _service.GetConvertorMapToBaseCurrencyAsync(new GetConvertorMapToBaseCurrencyRequest()
            {
                BaseAsset = "ALGO",
                BrokerId = Broker
            }).GetAwaiter().GetResult();

            foreach (var map in result.Maps)
            {
                Console.Write($"{map.AssetSymbol} to ALGO: ");
                foreach (var item in map.Operations.OrderBy(e => e.Order))
                {
                    Console.Write($"{item.InstrumentPrice}({(item.IsMultiply ? "*" : "/")}) ; ");
                }

                Console.WriteLine();
            }
        }



        private void AddAsset(string symbol)
        {
            _assets.Assets.Add(new Asset()
            {
                BrokerId = Broker,
                Symbol = symbol,
                IsEnabled = true
            });
        }

        private void AddInstrument(string symbol, string baseAsset, string quoteAsset)
        {
            _instruments.Instruments.Add(new SpotInstrument()
            {
                BrokerId = Broker,
                Symbol = symbol,
                BaseAsset = baseAsset,
                QuoteAsset = quoteAsset,
                IsEnabled = true
            });
        }
    }
}
