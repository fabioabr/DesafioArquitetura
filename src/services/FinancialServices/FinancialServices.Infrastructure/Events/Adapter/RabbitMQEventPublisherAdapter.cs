
using FinancialServices.Utils;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Events.Adapter
{
    public class RabbitMQEventPublisherAdapter : IEventPublisherAdapter
    {
        protected readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private readonly IConnection connection;        
        private readonly Dictionary<string, IChannel> channels;        
        
        private readonly ILogger logger;

        public RabbitMQEventPublisherAdapter(IConnection connection, ILogger logger)
        {
            this.logger = logger;
            this.connection = connection;            
            this.channels = new Dictionary<string, IChannel>();

           
        }
        public void Publish<T>(T document, string? routingKey, Dictionary<string,object>? attributes)
        {
            // Obter o nome do tópico a partir do atributo da classe T
            var t = typeof(T);

            var attr = Attribute.GetCustomAttribute(t, typeof(EventIdentificationAttribute));
            
            if (attr != null)
            {
                var a = (EventIdentificationAttribute)attr;
                var exchangeName = a.Topic;
                var exchangeType = "topic";

                var channel = GetChannel(exchangeName, exchangeType);
                
                string serializedMessage = JsonSerializer.Serialize(document, this.jsonSerializerOptions);

                byte[] body = Encoding.UTF8.GetBytes(serializedMessage);

                BasicProperties properties = new BasicProperties();

                if (attributes != null)
                    properties = new BasicProperties{Headers = attributes!};
                                
                routingKey ??= string.Empty;

                channel.BasicPublishAsync(exchangeName, routingKey, false, properties, body).GetAwaiter().GetResult();

            }
            else
            {
                throw new Exception("EventModelTopicAttribute is required for RabbitMQ event publishing");
            }
             
        }

        private IChannel GetChannel(string exchangeName, string exchangeType)
        {
            if (!channels.ContainsKey(exchangeName))
            {                
                var channel = connection.CreateChannelAsync()
                    .GetAwaiter().GetResult();

                channel.ExchangeDeclareAsync(
                    exchange: exchangeName,
                    type: exchangeType,
                    durable: true,
                    autoDelete: false
                ).GetAwaiter().GetResult();

                channels.Add(exchangeName, channel);
            }
                
            return channels[exchangeName];
            
        }
        
    }

}
