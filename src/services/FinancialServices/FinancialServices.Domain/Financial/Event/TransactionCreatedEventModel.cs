using FinancialServices.Domain.Financial.Model;
using FinancialServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Event
{
    
    [EventModelTopic(Topic = "TransactionCreated")]
    public class TransactionCreatedEventModel : TransactionModel
    {


    }
}
