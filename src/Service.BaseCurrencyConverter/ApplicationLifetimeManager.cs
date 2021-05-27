using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.AssetsDictionary.Client;
using Service.BaseCurrencyConverter.Domain.Models;

namespace Service.BaseCurrencyConverter
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlTcpClient _myNoSqlTcpClient;
        

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, MyNoSqlTcpClient myNoSqlTcpClient)
            : base(appLifetime)
        {
            _logger = logger;
            _myNoSqlTcpClient = myNoSqlTcpClient;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myNoSqlTcpClient.Start();
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
