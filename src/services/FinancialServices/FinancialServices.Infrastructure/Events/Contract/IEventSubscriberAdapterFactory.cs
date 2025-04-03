
using FinancialServices.Infrastructure.Enums;
using FinancialServices.Infrastructure.Events.Adapter;
using Microsoft.Extensions.Logging;

namespace FinancialServices.Infrastructure.Data.Contract
{
    public interface IEventSubscriberAdapterFactory
    {
        IEventSubscriberAdapter CreateEventSubscriberAdapter(ILogger logger);

    }
}
