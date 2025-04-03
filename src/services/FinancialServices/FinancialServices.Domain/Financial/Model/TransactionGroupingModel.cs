using FinancialServices.Domain.Financial.Enum;

namespace FinancialServices.Domain.Financial.Model
{
    public class TransactionGroupingModel
    {
        public  Guid Id { get; set; } = Guid.NewGuid();
        public  DateTime Period { get; set; } = DateTime.UtcNow;
        public  DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public  int TransactionCount { get; set; } = 0;
        public  decimal TotalAmount { get; set; } = 0;
        public  TransactionTypeEnum TransactionType { get; set; }

    }

}
