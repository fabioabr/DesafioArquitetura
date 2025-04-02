using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Model
{
    public class ConsolidatedReportModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public DateTime Date { get; set; }
        public List<ConsolidatedReportItemModel> Items { get; set; } = [];

    }

}
