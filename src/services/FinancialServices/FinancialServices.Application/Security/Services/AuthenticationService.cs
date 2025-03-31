using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;
using Microsoft.Extensions.Logging;

namespace FinancialServices.Application.Security.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<UserEntity> userRepository;
        private readonly ILogger logger;

        public AuthenticationService(IRepository<UserEntity> userRepository, ILogger logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
        }

        public bool AuthenticateByApiKey(string apiKey)
        {
            var user = userRepository.Query()
              .Where(p => p.ApiKey == apiKey)
              .FirstOrDefault();

            if (user == null)            
                logger.LogWarning("Usuário não encontrado para a chave de API fornecida.");
             
            return user != null;
        }
    }
}
