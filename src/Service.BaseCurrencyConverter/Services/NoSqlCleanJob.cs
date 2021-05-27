using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;

namespace Service.BaseCurrencyConverter.Services
{
    public class NoSqlCleanJob
    {
        private readonly IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> _dataWriter;

        public NoSqlCleanJob(IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql> dataWriter, IAssetsDictionaryClient assetsDictionary, ISpotInstrumentDictionaryClient instrumentDictionary)
        {
            _dataWriter = dataWriter;
            assetsDictionary.OnChanged += AssetsAndInstrumentDictionary_OnChanged;
            instrumentDictionary.OnChanged += AssetsAndInstrumentDictionary_OnChanged;
        }

        private void AssetsAndInstrumentDictionary_OnChanged()
        {
            _dataWriter.CleanAndKeepMaxPartitions(0);
        }
    }
}
