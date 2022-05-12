using Autofac;
using MyJetWallet.Sdk.NoSql;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Services;

namespace Service.BaseCurrencyConverter.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var myNoSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);

            builder.RegisterAssetsDictionaryClients(myNoSqlClient);

            builder.RegisterMyNoSqlWriter<BaseAssetConvertMapNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), BaseAssetConvertMapNoSql.TableName, true);

            builder
                .RegisterType<BaseCurrencyConverterJob>()
                .AsSelf()
                .SingleInstance();
        }
    }
}