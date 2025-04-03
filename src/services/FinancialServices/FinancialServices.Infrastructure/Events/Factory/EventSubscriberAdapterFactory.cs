using FinancialServices.Domain.Model;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Enums;
using FinancialServices.Infrastructure.Events.Adapter;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FinancialServices.Infrastructure.Data.Factory
{
    public class EventSubscriberAdapterFactory : IEventSubscriberAdapterFactory
    {
        private readonly ApplicationSettingsModel settings;
        public EventSubscriberAdapterFactory(ApplicationSettingsModel settings)
        {
            this.settings = settings;
        }
       
        public IEventSubscriberAdapter CreateEventSubscriberAdapter(ILogger logger)
        {
            var eventBusType = Enum.Parse<EventBusTypeEnum>(settings.EventBusToUse);

            if (eventBusType == EventBusTypeEnum.RabbitMQ)
            {

                string hostname = settings.EventBusSettings.RabbitMQSettings.HostName;
                int port = settings.EventBusSettings.RabbitMQSettings.Port;
                string user = settings.EventBusSettings.RabbitMQSettings.User;
                string password = settings.EventBusSettings.RabbitMQSettings.Password;
                string exchangeType = settings.EventBusSettings.RabbitMQSettings.ExchangeType;

                var connectionFactory = new ConnectionFactory()
                {
                    HostName = hostname,
                    UserName = user,
                    Password = password,
                    Port = port,
                    Ssl = new SslOption() { Enabled = false }
                };

                var connection = connectionFactory.CreateConnectionAsync()
                   .GetAwaiter().GetResult();

                return new RabbitMQEventSubscriberAdapter(connection, logger);

            }
            else
            {
                throw new NotImplementedException("EventBusType not implemented");
            }
        }
    }

}
