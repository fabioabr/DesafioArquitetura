using AutoMapper;
using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;

namespace FinancialServices.Application.Financial.Mapper
{
    public class TransactionModelMappingProfile : Profile
    {
        public TransactionModelMappingProfile()
        {
            CreateMap<TransactionModel, TransactionEntity>().ReverseMap();
            CreateMap<TransactionModel, TransactionCreatedEventModel>().ReverseMap();
        }

    }
}
