using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.BaseCurrencyConverter.Grpc;

namespace Service.BaseCurrencyConverter.Client
{
    [UsedImplicitly]
    public class BaseCurrencyConverterClientFactory
    {
        private readonly CallInvoker _channel;

        public BaseCurrencyConverterClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IBaseCurrencyConverterService GetBaseCurrencyConverterService() => _channel.CreateGrpcService<IBaseCurrencyConverterService>();
    }
}
