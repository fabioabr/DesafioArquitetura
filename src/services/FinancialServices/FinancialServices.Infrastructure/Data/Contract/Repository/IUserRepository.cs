using FinancialServices.Infrastructure.Data.Attributes;
using FinancialServices.Infrastructure.Data.Concrete.Entity;
using FinancialServices.Infrastructure.Data.Contract.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialServices.Infrastructure.Data.Contract.Repository
{
    
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> GetByApiKeyAsync(string apiKey);
        Task<UserEntity?> GetByUsername(string userName);
       
    }
}
