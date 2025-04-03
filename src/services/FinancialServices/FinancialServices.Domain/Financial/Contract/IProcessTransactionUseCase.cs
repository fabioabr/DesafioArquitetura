using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Security.Model;
using FinancialServices.Utils.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Contract
{
    public interface IProcessTransactionUseCase
    {
        Task<GenericResponse> ProcessTransaction(TransactionCreatedEventModel transaction);


    }
}
