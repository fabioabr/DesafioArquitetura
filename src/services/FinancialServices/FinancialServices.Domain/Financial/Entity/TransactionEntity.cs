using FinancialServices.Domain.Core.Attributes;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Model
{
    [EntitySetName("Transactions")]
    public class TransactionEntity : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required TransactionTypeEnum Type { get; set; } 
        public required decimal Amount { get; set; }
        public required string Description { get; set; }
        public required DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required string SourceAccount { get; set; }
        public required string DestinationAccount { get; set; }
        public Guid? OriginalTransactionId { get; set; }

    }
}
