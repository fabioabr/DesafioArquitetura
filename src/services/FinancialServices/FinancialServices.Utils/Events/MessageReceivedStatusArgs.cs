namespace FinancialServices.Utils.Events
{

    public class MessageReceivedStatusArgs
    {
        public  bool Ack { get; set; } = false;
        public  bool Requeue { get; set; } = false;
        public ulong DeliveryTag { get; set; } = 0;

    }
}
