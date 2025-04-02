using FinancialServices.Domain.Financial.Enum;
using FinancialServices.Domain.Financial.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Service
{
    public class TransactionValidationService : ITransactionValidationService
    {
        public void ValidateTransaction(TransactionModel transaction)
        {
            // Validações
            if (transaction.Amount <= 0)
                throw new Exception("Amount must be greater than 0");
            if (string.IsNullOrWhiteSpace(transaction.SourceAccount))
                throw new Exception("SourceAccount is required");
            if (string.IsNullOrWhiteSpace(transaction.DestinationAccount))
                throw new Exception("DestinationAccount is required");
            if (transaction.SourceAccount == transaction.DestinationAccount)
                throw new Exception("SourceAccount and DestinationAccount must be different");                        
            if (transaction.Type == TransactionTypeEnum.Refund && !transaction.OriginalTransactionId.HasValue)
                throw new Exception("OriginalTransactionId is required for Refund transactions");

        }

    }
}
