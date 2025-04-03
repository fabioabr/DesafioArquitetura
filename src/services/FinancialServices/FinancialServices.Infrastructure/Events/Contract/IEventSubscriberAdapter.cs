using FinancialServices.Utils.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace FinancialServices.Infrastructure.Events.Adapter
{
    public interface IEventSubscriberAdapter
    {
        public Task SubscribeAsync<T>(string topic, string subscription, Func<T, MessageReceivedStatusArgs, Task> onMessage, CancellationToken cancellationToken = default);


    }
}
