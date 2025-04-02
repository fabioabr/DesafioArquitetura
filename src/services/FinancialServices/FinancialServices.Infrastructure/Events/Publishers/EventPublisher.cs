using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Infrastructure.Events.Adapter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Events.Publishers
{
    public class EventPublisher<T> : IEventPublisher<T> where T : class
    {
        private protected IEventPublisherAdapter eventPublisherAdapter;
                
        public EventPublisher(IEventPublisherAdapter eventPublisherAdapter)
        {
            this.eventPublisherAdapter = eventPublisherAdapter;            
        }
        
        public void Publish(T document)
        {
            Publish(document, null, null);
        }

        public void Publish(T document, string? routingKey)
        {
            Publish(document, routingKey, null);
        }
        public void Publish(T document, Dictionary<string, object>? attributes)
        {
            Publish(document, null, attributes);
        }

        public void Publish(T document, string? routingKey, Dictionary<string, object>? attributes)
        {
            eventPublisherAdapter.Publish(document, routingKey, attributes);
        }


    }
}
