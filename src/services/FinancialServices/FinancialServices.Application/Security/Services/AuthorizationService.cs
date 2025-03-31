using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;
using Microsoft.Extensions.Logging;

namespace FinancialServices.Application.Security.Services
{
    public class AuthorizationService : IAuthorizationService
    {        
        private readonly ILogger logger;
        private readonly IRepository<UserEntity> userRepository;
        public AuthorizationService(ILogger logger, IRepository<UserEntity> userRepository)
        {            
            this.logger = logger;
            this.userRepository = userRepository;
        }

        public bool AuthorizeUserByApiKey(string apiKey, string[] requiredRoles)
        {
            var user = userRepository.Query()
                .Where(p => p.ApiKey == apiKey)
                .FirstOrDefault();

            if (user == null)
                logger.LogWarning("Usuário não encontrado para a chave de API fornecida.");
            
            if (requiredRoles == null || !requiredRoles.Any())
                return true;

            if (user!.Roles == null || !user.Roles.Any())
            {
                logger.LogWarning("Usuário não possui nenhuma role atribuída.");
                return false;
            }

            var hasRequiredRole = user.Roles
                .Any(r => requiredRoles.Any(rr => rr.Equals(r, StringComparison.OrdinalIgnoreCase)));

            if (!hasRequiredRole)
            {
                logger.LogWarning("Usuário não possui roles necessárias para utilizar o endpoint.");
                return false;
            }

            logger.LogInformation("Usuário autenticado e autorizado com sucesso.");

            return true;

        }
    }
}
