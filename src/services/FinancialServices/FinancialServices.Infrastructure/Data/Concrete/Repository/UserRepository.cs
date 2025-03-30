using FinancialServices.Infrastructure.Data.Concrete.Entity;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Data.Contract.Repository;
using MongoDB.Driver.Linq;

namespace FinancialServices.Infrastructure.Data.Concrete
{
    public class UserRepository : Repository<UserEntity>, IUserRepository
    {
        public UserRepository() 
        {
            
        }

        public UserRepository(IDatabaseAdapter databaseAdapter) : base(databaseAdapter)
        {
                
        }

        public async Task<UserEntity?> GetByApiKeyAsync(string apiKey)
        {
            return await Query()
                .Where(p=>p.ApiKey == apiKey)
                .FirstOrDefaultAsync();
             
        }

       

        public async Task<UserEntity?> GetByUsername(string userName)
        {
            return await Query()
               .Where(p => p.UserName == userName)
               .FirstOrDefaultAsync();
        }
    }
}
