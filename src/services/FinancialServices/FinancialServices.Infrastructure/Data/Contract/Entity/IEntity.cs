using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Data.Contract.Entity
{
    public interface IEntity
    {
        public Guid Id { get; set; }

    }
}
