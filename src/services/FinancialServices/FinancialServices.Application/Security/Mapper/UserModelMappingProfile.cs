using AutoMapper;
using FinancialServices.Domain.Security.Model;
using FinancialServices.Infrastructure.Data.Concrete.Entity;

namespace FinancialServices.Application.Security.Mapper
{
    public class UserModelMappingProfile : Profile
    {
        public UserModelMappingProfile()
        {
            CreateMap<UserEntity, UserModel>().ReverseMap();            
        }

    }
}
