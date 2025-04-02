using FinancialServices.Infrastructure.Enums;

namespace FinancialServices.Api.Model
{
     
    public class ApplicationSettingsModel
    {
        public bool UseTransactionEndpoints { get; set; } = true;
        public bool UseReportEndpoints { get; set; } = true;
        public bool UseConsolidationReportJob { get; set; } = true;                   
        public DatabaseTypeEnum DatabaseToUse { get; set; } = DatabaseTypeEnum.InMemorySQLite;
        public EventBusTypeEnum EventBusToUse { get; set; } = EventBusTypeEnum.RabbitMQ;
                
        public DatabaseSettings DatabasSettingse { get; set; } = new();
        public ObservabilitySettings ObservabilitySettings { get; set; } = new();
        public EventBusSettings EventBusSettings { get; set; } = new();
        public CronJobSettings JobsSettings { get; set; } = new();
    }

    public class DatabaseSettings
    {
        public Dictionary<string,string> ConnectionStrings { get; set; } = [];
    }

    public class ObservabilitySettings
    {
        public string GrafanaLokiUrl { get; set; } = string.Empty;
    }

    public class ConnectionStringsSettings
    {        
        public string MongoDb { get; set; } = string.Empty;
    }

    public class CronJobSettings
    {
        public CreateReportJobSettings CreateReportsJob { get; set; } = new();

    }
     
    public class CreateReportJobSettings
    {
        public string CronScheduleConfig { get; set; } = "0 */20 * * * ?";
        public string TimezoneOffsets { get; set; } = "0, -3";
    }

    public class EventBusSettings
    {
        public RabbitMQSettings RabbitMQSettings { get; set; } = new();
       
    }

    public class RabbitMQSettings
    {
        /*
          - CustomSettings__EventBusSettings__RabbitMQSettings__HostName=rabbitmq
          - CustomSettings__EventBusSettings__RabbitMQSettings__Port=5672
          - CustomSettings__EventBusSettings__RabbitMQSettings__User=admin
          - CustomSettings__EventBusSettings__RabbitMQSettings__Password=admin
          - CustomSettings__EventBusSettings__RabbitMQSettings__ExchangeType=topic
        */

        public string HostName { get; set; } = "rabbitmq";
        public string User { get; set; } = "admin";
        public string Password { get; set; } = "admin";
        public int Port { get; set; } = 5672;
        public string ExchangeType { get; set; } = "topic";
    }

}
