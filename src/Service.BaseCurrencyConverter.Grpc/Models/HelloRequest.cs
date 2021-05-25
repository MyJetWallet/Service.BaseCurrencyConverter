using System.Runtime.Serialization;

namespace Service.BaseCurrencyConverter.Grpc.Models
{
    [DataContract]
    public class GetConvertorMapToBaseCurrencyRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string BaseAsset { get; set; }
    }
}