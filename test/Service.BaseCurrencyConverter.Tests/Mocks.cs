using System;
using System.Collections.Generic;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Client;
using Service.AssetsDictionary.Domain.Models;

namespace Service.BaseCurrencyConverter.Tests
{
    public class AssetsDictionaryClientMock: IAssetsDictionaryClient
    {
        public IAsset GetAssetById(IAssetIdentity assetId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IAsset> GetAssetsByBroker(IJetBrokerIdentity brokerId)
        {
            throw new NotImplementedException();
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
}