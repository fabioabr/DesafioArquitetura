using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Events.Adapter
{
    public interface IEventPublisherAdapter
    {
        void Publish<T>(T document, string? routingKey, Dictionary<string, object>? attributes);
    }
}
