using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Entity;

namespace FinancialServices.Domain.Security.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<UserEntity> userRepository;
        
        public AuthenticationService(IRepository<UserEntity> userRepository)
        {
            this.userRepository = userRepository;         
        }

        public bool AuthenticateByApiKey(string apiKey)
        {
            var user = userRepository.Query()
              .Where(p => p.ApiKey == apiKey)
              .FirstOrDefault();

           
            return user != null;
        }
    }
}
