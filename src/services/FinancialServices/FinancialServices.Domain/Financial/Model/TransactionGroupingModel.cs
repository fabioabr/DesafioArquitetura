using FinancialServices.Domain.Financial.Enum;

namespace FinancialServices.Domain.Financial.Model
{
    public class TransactionGroupingModel
    {
        public required Guid Id { get; set; }
        public required DateTime Period { get; set; } = DateTime.UtcNow;
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required int TransactionCount { get; set; } = 0;
        public required decimal TotalAmount { get; set; } = 0;
        public required TransactionTypeEnum TransactionType { get; set; }

    }

}
