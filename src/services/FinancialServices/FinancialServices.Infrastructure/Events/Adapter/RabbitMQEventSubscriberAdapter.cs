using FinancialServices.Utils.Events;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Core;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace FinancialServices.Infrastructure.Events.Adapter
{
    public class RabbitMQEventSubscriberAdapter : IEventSubscriberAdapter
    {
        protected readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private readonly ILogger logger;
        private readonly IConnection connection;

        public RabbitMQEventSubscriberAdapter(IConnection connection, ILogger logger)
        {
            this.connection =  connection;
            this.logger = logger;
        }
         
        public async Task SubscribeAsync<T>(string topic, string subscription, Func<T?, MessageReceivedStatusArgs, Task> onMessage, CancellationToken cancellationToken = default)
        {

            var channel = await connection.CreateChannelAsync();
            
            // Garante que o exchange e a fila existem
            await channel.ExchangeDeclareAsync(exchange: topic, type: ExchangeType.Topic, durable: true);
            await channel.QueueDeclareAsync(queue: subscription, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: subscription, exchange: topic, routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();

                    var message = Encoding.UTF8.GetString(body);

                    logger.LogInformation("Received message from RabbitMQ: {Message}", message);

                    var obj = JsonSerializer.Deserialize<T>(message, jsonSerializerOptions);

                    var messageReceivedStatus = new MessageReceivedStatusArgs()
                    {
                        DeliveryTag = ea.DeliveryTag,
                    };

                    await onMessage(obj, messageReceivedStatus);

                    if (messageReceivedStatus.Ack)
                    {
                        // ACK manual da mensagem
                        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        // NACK manual da mensagem
                        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: messageReceivedStatus.Requeue);
                    }

                }
                catch (JsonException ex)
                {                    
                    logger.LogError($"Erro ao desserializar mensagem: {ex.Message}");                 
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
                catch (Exception ex)
                {                    
                    logger.LogError($"Erro ao processar mensagem: {ex.Message}");                    
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync(queue: subscription, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        
    }

}
