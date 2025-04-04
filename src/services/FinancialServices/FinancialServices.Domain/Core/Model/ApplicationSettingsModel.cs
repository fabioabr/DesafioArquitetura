
namespace FinancialServices.Domain.Model
{
     
    public class ApplicationSettingsModel
    {
        public bool UseTransactionEndpoints { get; set; } = true;        
        public bool UseConsolidationReportJob { get; set; } = true;
        public bool UseDevelopmentTransactionBigSeed{ get; set; } = false;
        public bool UseDevelopmentTransactionContinuousSeed { get; set; } = false;
        
        public bool UseSubscriptions { get; set; } = false;
        public string DatabaseToUse { get; set; } = "InMemorySQLite";
        public string EventBusToUse { get; set; } = "RabbitMQ";
                
        public DatabaseSettings DatabasSettings { get; set; } = new();
        public ObservabilitySettings ObservabilitySettings { get; set; } = new();
        public EventBusSettings EventBusSettings { get; set; } = new();
        public CronJobSettings JobsSettings { get; set; } = new();
    }

    public class DatabaseSettings
    {
        public MongoDbSettings MongoDbSettings {get;set;} = new ();
       
    }
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = "mongodb://admin:admin@mongodb:27017/FinancialDB?authSource=admin";
        public string DatabaseName { get; set; } = "FinancialDB";
    }

    public class ObservabilitySettings
    {
        public string GrafanaLokiUrl { get; set; } = string.Empty;
    }

    public class ConnectionStringsSettings
    {        
        public string MongoDB { get; set; } = string.Empty;
    }

    public class CronJobSettings
    {
        public CreateReportJobSettings CreateReportsJob { get; set; } = new();

    }
     
    public class CreateReportJobSettings
    {
        public string CronScheduleConfig { get; set; } = "0 */20 * * * ?";
        public string[] Timezones { get; set; } = ["UTC, America/Sao_Paulo"];
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
