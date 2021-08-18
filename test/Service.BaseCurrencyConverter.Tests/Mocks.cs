using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.AssetsDictionary.Domain.Models;
using Service.BaseCurrencyConverter.Domain.Models;

namespace Service.BaseCurrencyConverter.Tests
{
    public class AssetsDictionaryClientMock: IAssetsDictionaryClient
    {
        public List<Asset> Assets = new List<Asset>();

        public IAsset GetAssetById(IAssetIdentity assetId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IAsset> GetAssetsByBroker(IJetBrokerIdentity brokerId)
        {
            return Assets.Where(e => e.BrokerId == brokerId.BrokerId).ToList();
        }

        public IReadOnlyList<IAsset> GetAssetsByBrand(IJetBrandIdentity brandId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IAsset> GetAllAssets()
        {
            throw new NotImplementedException();
        }

        public event Action OnChanged;
    }

    public class SpotInstrumentDictionaryClientMock : ISpotInstrumentDictionaryClient
    {
        public List<SpotInstrument> Instruments = new List<SpotInstrument>();

        public ISpotInstrument GetSpotInstrumentById(ISpotInstrumentIdentity spotInstrumentId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ISpotInstrument> GetSpotInstrumentByBroker(IJetBrokerIdentity brokerId)
        {
            return Instruments.Where(e => e.BrokerId == brokerId.BrokerId).ToList();
        }

        public IReadOnlyList<ISpotInstrument> GetSpotInstrumentByBrand(IJetBrandIdentity brandId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ISpotInstrument> GetAllSpotInstruments()
        {
            throw new NotImplementedException();
        }

        public event Action OnChanged;
    }

    public class MyNoSqlServerDataWriterBaseAssetConvertMapNoSqlMock : IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql>
    {
        public ValueTask InsertAsync(BaseAssetConvertMapNoSql entity)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask InsertOrReplaceAsync(BaseAssetConvertMapNoSql entity)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepLastRecordsAsync(string partitionKey, int amount)
        {
            throw new NotImplementedException();
        }

        public ValueTask BulkInsertOrReplaceAsync(IEnumerable<BaseAssetConvertMapNoSql> entity, DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndBulkInsertAsync(IEnumerable<BaseAssetConvertMapNoSql> entity, DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndBulkInsertAsync(string partitionKey, IEnumerable<BaseAssetConvertMapNoSql> entity,
            DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask<OperationResult> ReplaceAsync(string partitionKey, string rowKey, Func<BaseAssetConvertMapNoSql, bool> updateCallback,
            DataSynchronizationPeriod syncPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask<OperationResult> MergeAsync(string partitionKey, string rowKey, Func<BaseAssetConvertMapNoSql, bool> updateCallback,
            DataSynchronizationPeriod syncPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<BaseAssetConvertMapNoSql>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<BaseAssetConvertMapNoSql>> GetAsync(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<BaseAssetConvertMapNoSql> GetAsync(string partitionKey, string rowKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IReadOnlyList<BaseAssetConvertMapNoSql>> GetMultipleRowKeysAsync(string partitionKey, IEnumerable<string> rowKeys)
        {
            throw new NotImplementedException();
        }

        public ValueTask<BaseAssetConvertMapNoSql> DeleteAsync(string partitionKey, string rowKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<BaseAssetConvertMapNoSql>> QueryAsync(string query)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<BaseAssetConvertMapNoSql>> GetHighestRowAndBelow(string partitionKey, string rowKeyFrom, int amount)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepMaxPartitions(int maxAmount)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepMaxRecords(string partitionKey, int maxAmount)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> GetCountAsync(string partitionKey)
        {
            throw new NotImplementedException();
        }
    }
}