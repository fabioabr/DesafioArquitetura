using FinancialServices.Domain.Financial.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Service
{
    public interface ITransactionValidationService
    {
        void ValidateTransaction(TransactionModel transaction);

    }
}
