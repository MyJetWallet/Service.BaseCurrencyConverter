using System;
using Elasticsearch.Net.Specification.IndicesApi;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Service.AssetsDictionary.Domain.Models;
using Service.BaseCurrencyConverter.Services;
using Service.BaseCurrencyConverter.Settings;

namespace Service.BaseCurrencyConverter.Tests
{
    public class TestExample
    {
        private AssetsDictionaryClientMock _assets = new AssetsDictionaryClientMock();
        private SpotInstrumentDictionaryClientMock _instruments = new SpotInstrumentDictionaryClientMock();
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

            _service = new BaseCurrencyConverterService(loggerFactory.CreateLogger<BaseCurrencyConverterService>(), _assets, _instruments, _settings, _writer);
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine("Debug output");
            Assert.Pass();
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
