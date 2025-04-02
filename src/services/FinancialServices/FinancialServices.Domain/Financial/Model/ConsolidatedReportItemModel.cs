using FinancialServices.Domain.Financial.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Model
{
    public class ConsolidatedReportItemModel
    {
        public required DateTime Start { get; set; } = DateTime.UtcNow;
        public required DateTime End { get; set; } = DateTime.UtcNow;
        public required int TransactionCount { get; set; } = 0;
        public required decimal TotalAmount { get; set; } = 0;
        public required TransactionTypeEnum TransactionType { get; set; }


    }
}
