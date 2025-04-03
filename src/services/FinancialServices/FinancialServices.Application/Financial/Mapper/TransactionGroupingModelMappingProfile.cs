using AutoMapper;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;

namespace FinancialServices.Application.Financial.Mapper
{
    public class TransactionGroupingModelMappingProfile : Profile
    {
        public TransactionGroupingModelMappingProfile()
        {
            CreateMap<TransactionGroupingModel, TransactionGroupingEntity>().ReverseMap();            
        }

    }
}
