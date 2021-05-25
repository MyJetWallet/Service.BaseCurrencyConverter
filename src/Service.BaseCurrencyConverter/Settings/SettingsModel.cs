using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.BaseCurrencyConverter.Settings
{
    public class SettingsModel
    {
        [YamlProperty("BaseCurrencyConverter.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BaseCurrencyConverter.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("BaseCurrencyConverter.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("BaseCurrencyConverter.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("BaseCurrencyConverter.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("BaseCurrencyConverter.CrossAssetsList")]
        public string CrossAssetsList { get; set; }
    }
}
