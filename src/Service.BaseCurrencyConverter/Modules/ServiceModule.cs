using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Grpc.Core.Logging;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using MyNoSqlServer.DataWriter;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;

namespace Service.BaseCurrencyConverter.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var myNoSqlClient = new MyNoSqlTcpClient(
                Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort),
                ApplicationEnvironment.HostName ??
                $"{ApplicationEnvironment.AppName}:{ApplicationEnvironment.AppVersion}");

            builder.RegisterInstance(myNoSqlClient).AsSelf().SingleInstance();

            builder.RegisterType<MyNoSqlTcpClientWatcher>().AutoActivate().SingleInstance();

            builder.RegisterAssetsDictionaryClients(myNoSqlClient);

            builder
                .RegisterInstance(new MyNoSqlServerDataWriter<BaseAssetConvertMapNoSql>(
                    Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), BaseAssetConvertMapNoSql.TableName, true))
                .As<IMyNoSqlServerDataWriter<BaseAssetConvertMapNoSql>>()
                .SingleInstance();
        }
    }

    public class MyNoSqlTcpClientWatcher: IStartable, IDisposable
    {
        private readonly MyNoSqlTcpClient _myNoSqlTcpClient;
        private readonly ILogger<MyNoSqlTcpClientWatcher> _logger;
        private readonly MyTaskTimer _timer;

        public MyNoSqlTcpClientWatcher(MyNoSqlTcpClient myNoSqlTcpClient, ILogger<MyNoSqlTcpClientWatcher> logger)
        {
            _myNoSqlTcpClient = myNoSqlTcpClient;
            _logger = logger;

            _timer = new MyTaskTimer(nameof(MyNoSqlTcpClientWatcher), TimeSpan.FromSeconds(10), logger, Watch);
        }

        public void Start()
        {
            _timer.Start();
        }

        private Task Watch()
        {
            if (!_myNoSqlTcpClient.Connected)
                _logger.LogError("MyNoSqlTcpClient DO NOT CONNECTED, please start the client and validate url and connection");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}