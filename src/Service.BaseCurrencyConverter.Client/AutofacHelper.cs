using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using MyNoSqlServer.DataWriter;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.BaseCurrencyConverter.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBaseCurrencyConverterClient(this ContainerBuilder builder, string grpcServiceUrl, MyNoSqlTcpClient myNoSqlTcpClient)
        {
            var factory = new BaseCurrencyConverterClientFactory(grpcServiceUrl);
            var service = factory.GetBaseCurrencyConverterService();

            var reader = builder.RegisterMyNoSqlReader<BaseAssetConvertMapNoSql>(myNoSqlTcpClient, BaseAssetConvertMapNoSql.TableName);

            builder
                .RegisterInstance(new BaseCurrencyConverterClientWithCache(service, reader))
                .As<IBaseCurrencyConverterService>()
                .SingleInstance();
        }
    }
}
