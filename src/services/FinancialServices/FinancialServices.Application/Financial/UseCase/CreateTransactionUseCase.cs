using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Financial.Service;
using FinancialServices.Utils.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Application.Financial.UseCase
{
    public class CreateTransactionUseCase : ICreateTransactionUseCase
    {
        
        private readonly ITransactionValidationService transactionValidationService;
        private readonly IMapper mapper;
        private readonly IEventPublisher<TransactionCreatedEventModel> transactionModelEventPublisher;

        public CreateTransactionUseCase(ITransactionValidationService transactionValidationService, IEventPublisher<TransactionCreatedEventModel> transactionModelEventPublisher, IMapper mapper)
        {            
            this.transactionValidationService = transactionValidationService;
            this.mapper = mapper;
            this.transactionModelEventPublisher = transactionModelEventPublisher;
        }

        public GenericResponse<TransactionModel?> CreateTransaction (TransactionModel transaction)
        {

            try
            {
                transactionValidationService.ValidateTransaction(transaction);

                var transactionEntity =  mapper.Map<TransactionEntity>(transaction);

                transactionModelEventPublisher.Publish(mapper.Map<TransactionCreatedEventModel>(transaction));

                return new GenericResponse<TransactionModel?>
                {
                    Success = true,
                    Data = transaction,                    
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<TransactionModel?> { 
                    Success = false,
                    Data = transaction,
                    Message = ex.Message                 
                };
            }

        }
    }
}
