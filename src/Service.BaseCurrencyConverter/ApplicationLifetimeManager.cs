using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;
using Service.BaseCurrencyConverter.Services;

namespace Service.BaseCurrencyConverter
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlClientLifeTime _myNoSqlTcpClient;
        private readonly BaseCurrencyConverterJob _baseCurrencyConverterJob;


        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, 
            ILogger<ApplicationLifetimeManager> logger, 
            MyNoSqlClientLifeTime myNoSqlTcpClient, 
            BaseCurrencyConverterJob baseCurrencyConverterJob)
            : base(appLifetime)
        {
            _logger = logger;
            _myNoSqlTcpClient = myNoSqlTcpClient;
            _baseCurrencyConverterJob = baseCurrencyConverterJob;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myNoSqlTcpClient.Start();
            _baseCurrencyConverterJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
