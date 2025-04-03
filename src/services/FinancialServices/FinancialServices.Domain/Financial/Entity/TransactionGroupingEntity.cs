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
        public Guid Id { get ; set ; } = Guid.NewGuid();
        public DateTime Period { get; set; } = DateTime.UtcNow;        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TransactionCount { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public TransactionTypeEnum TransactionType { get; set; }
        
    }

}
