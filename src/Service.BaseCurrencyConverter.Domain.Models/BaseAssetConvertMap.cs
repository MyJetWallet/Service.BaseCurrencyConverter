using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.BaseCurrencyConverter.Domain.Models
{
    [DataContract]
    public class BaseAssetConvertMap
    {
        [DataMember(Order = 1)] public string BaseAssetSymbol { get; set; }
        [DataMember(Order = 2)] public List<BaseAssetConvertMapItem> Maps { get; set; }

    }


    [DataContract]
    public class BaseAssetConvertMapItem
    {
        [DataMember(Order = 1)] public string AssetSymbol { get; set; }
        [DataMember(Order = 2)] public List<BaseAssetConvertMapOperation> Operations { get; set; }
    }

    [DataContract]
    public class BaseAssetConvertMapOperation
    {
        [DataMember(Order = 1)] public int Order { get; set; }
        [DataMember(Order = 2)] public bool IsMultiply { get; set; }
        [DataMember(Order = 3)] public string InstrumentPrice { get; set; }
        [DataMember(Order = 4)] public bool UseBid { get; set; }
    }
}