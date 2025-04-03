using FinancialServices.Domain.Core.Attributes;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Entity
{

    [EntitySetName("TransactionGroups")]
    public class TransactionGroupingEntity : IEntity
    {
        public required Guid Id { get ; set ; }
        public required DateTime Period { get; set; } = DateTime.UtcNow;        
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required int TransactionCount { get; set; } = 0;
        public required decimal TotalAmount { get; set; } = 0;
        public required TransactionTypeEnum TransactionType { get; set; }
        
    }

}
