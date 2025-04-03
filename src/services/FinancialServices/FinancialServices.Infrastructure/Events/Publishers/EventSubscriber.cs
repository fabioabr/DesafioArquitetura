using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Infrastructure.Events.Adapter;
using FinancialServices.Utils;
using FinancialServices.Utils.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Events.Publishers
{
    public class EventSubscriber<T> : IEventSubscriber<T> where T : class
    {
        private protected IEventSubscriberAdapter eventSubscriberAdapter;
        private protected ILogger logger;

 
        public EventSubscriber(IEventSubscriberAdapter eventSubscriberAdapter, ILogger logger)
        {
            this.eventSubscriberAdapter = eventSubscriberAdapter;
            this.logger = logger;
        }

        public virtual async Task StartListening(string subscriberId, Func<T, MessageReceivedStatusArgs, Task> onMessageReceived, CancellationToken cancellationToken = default)
        {
            var type = typeof(T);
            var attr = Attribute.GetCustomAttribute(type, typeof(EventIdentificationAttribute));

            if (attr == null)
                throw new ArgumentException($"Type {type.Name} does not have the EventModelTopicAttribute attribute.");

            var a = (EventIdentificationAttribute)attr;
            
            var exchangeName = a.Topic;
            var subscription = exchangeName + "." + subscriberId;

            await eventSubscriberAdapter.SubscribeAsync<T>(exchangeName, subscription, onMessageReceived);

                 

        }
    }
}
