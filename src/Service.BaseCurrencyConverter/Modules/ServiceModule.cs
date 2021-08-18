using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Grpc.Core.Logging;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using MyNoSqlServer.DataWriter;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Services;

namespace Service.BaseCurrencyConverter.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var myNoSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            builder.RegisterAssetsDictionaryClients(myNoSqlClient);

            builder.RegisterMyNoSqlWriter<BaseAssetConvertMapNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), BaseAssetConvertMapNoSql.TableName, true);

            builder.RegisterType<NoSqlCleanJob>().As<IStartable>().AutoActivate().SingleInstance();
        }
    }
}