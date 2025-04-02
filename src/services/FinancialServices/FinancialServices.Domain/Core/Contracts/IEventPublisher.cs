using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Core.Contracts
{
    public interface IEventPublisher<T> where T : class
    {
        void Publish(T document);
        void Publish(T document, string? routingKey);
        void Publish(T document, string? routingKey, Dictionary<string, object>? attributes);
        void Publish(T document, Dictionary<string, object>? attributes);

    }
}
