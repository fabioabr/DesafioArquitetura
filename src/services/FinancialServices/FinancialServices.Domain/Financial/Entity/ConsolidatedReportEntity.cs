using FinancialServices.Domain.Core.Attributes;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Entity
{
    [EntitySetName("Reports")]
    public class ConsolidatedReportEntity : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public DateTime Date { get; set; }
        public int TimezoneOffset { get; set; } = 0;
        public List<ConsolidatedReportItemModel> Items { get; set; } = [];

        

    }
}
