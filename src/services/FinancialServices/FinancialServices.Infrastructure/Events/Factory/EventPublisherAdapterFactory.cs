
using FinancialServices.Infrastructure.Data.Adapter;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Enums;
using FinancialServices.Infrastructure.Events.Adapter;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Data.Common;
using System.Threading.Channels;

namespace FinancialServices.Infrastructure.Data.Factory
{
    public class EventPublisherAdapterFactory : IEventPublisherAdapterFactory
    {
         
        public IEventPublisherAdapter CreateEventPublisherAdapter(EventBusTypeEnum eventBusType, ILogger logger)
        {

            if (eventBusType == EventBusTypeEnum.RabbitMQ)
            {

                string hostname = Environment.GetEnvironmentVariable("CustomSettings__EventBus__RabbitMQ__HostName") ?? throw new System.Exception("CustomSettings__EventBus__RabbitMQ__HostName is not set"); ;
                string port = Environment.GetEnvironmentVariable("CustomSettings__EventBus__RabbitMQ__Port") ?? throw new System.Exception("CustomSettings__EventBus__RabbitMQ__Port is not set"); ;
                string user = Environment.GetEnvironmentVariable("CustomSettings__EventBus__RabbitMQ__User") ?? throw new System.Exception("CustomSettings__EventBus__RabbitMQ__User is not set"); ;
                string password = Environment.GetEnvironmentVariable("CustomSettings__EventBus__RabbitMQ__Password") ?? throw new System.Exception("CustomSettings__EventBus__RabbitMQ__Password is not set"); ;
                string exchangeType = Environment.GetEnvironmentVariable("CustomSettings__EventBus__RabbitMQ__ExchangeType") ?? throw new System.Exception("CustomSettings__EventBus__RabbitMQ__ExchangeType is not set"); ;

                var connectionFactory = new ConnectionFactory()
                {
                    HostName = hostname,
                    UserName = user,
                    Password = password,
                    Port = int.Parse(port),
                    Ssl = new SslOption() { Enabled = false }
                };

                return new RabbitMQEventPublisherAdapter(connectionFactory);
                 
            }
            else
            {
                throw new NotImplementedException("EventBusType not implemented");
            }
             
        }
       
    }

}
