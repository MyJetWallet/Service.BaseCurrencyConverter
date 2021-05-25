using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Grpc.Models;

namespace Service.BaseCurrencyConverter.Grpc
{
    [ServiceContract]
    public interface IBaseCurrencyConverterService
    {
        [OperationContract]
        Task<BaseAssetConvertMap> GetConvertorMapToBaseCurrencyAsync(GetConvertorMapToBaseCurrencyRequest request);
    }
}