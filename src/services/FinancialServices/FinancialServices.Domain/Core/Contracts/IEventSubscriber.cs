using FinancialServices.Utils.Events;

namespace FinancialServices.Domain.Core.Contracts
{
    public interface IEventSubscriber<T> where T : class
    {
        Task StartListening(string subscriberId, Func<T, MessageReceivedStatusArgs, Task> onMessageReceived, CancellationToken cancellationToken = default);

    }
}
