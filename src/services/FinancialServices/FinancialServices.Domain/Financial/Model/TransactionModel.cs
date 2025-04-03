using FinancialServices.Domain.Financial.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Model
{
    public class TransactionModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public TransactionTypeEnum Type { get; set; } 
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string SourceAccount { get; set; } = string.Empty;
        public string DestinationAccount { get; set; } = string.Empty;
        public Guid? OriginalTransactionId { get; set; }

    }
}
