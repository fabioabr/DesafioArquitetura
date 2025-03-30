using AutoMapper;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;

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
