using FinancialServices.Infrastructure.Enums;

namespace FinancialServices.Api.Model
{
    public class ApplicationSettingsModel
    {
        public DatabaseTypeEnum DatabaseToUse { get; set; } = DatabaseTypeEnum.InMemorySQLite;
        public ObservabilitySettings Observability { get; set; } = new();
        public ConnectionStringsSettings ConnectionStrings { get; set; } = new();
    }

    public class ObservabilitySettings
    {
        public string GrafanaLokiUrl { get; set; } = string.Empty;
    }

    public class ConnectionStringsSettings
    {
        
        public string MongoDb { get; set; } = string.Empty;
    }
}
