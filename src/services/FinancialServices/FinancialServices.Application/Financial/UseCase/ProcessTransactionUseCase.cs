using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Utils.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Application.Financial.UseCase
{
    public class ProcessTransactionUseCase : IProcessTransactionUseCase
    {
        protected readonly IRepository<TransactionEntity> transactionRepository;
        protected readonly IMapper mapper;
        public ProcessTransactionUseCase(IRepository<TransactionEntity> transactionRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.mapper = mapper;
        }

        public async Task<GenericResponse> ProcessTransaction(TransactionCreatedEventModel transaction)
        {
            var response = new GenericResponse();

            try
            {             
                var transactionEntity = mapper.Map<TransactionEntity>(transaction);
                await transactionRepository.InsertOrUpdate(transactionEntity);
                return response.WithSuccess();
            }
            catch (Exception ex)
            {
                return response
                    .WithMessage(ex.Message)
                    .WithException(ex)
                    .WithFail();
            }
        }

    }
}
