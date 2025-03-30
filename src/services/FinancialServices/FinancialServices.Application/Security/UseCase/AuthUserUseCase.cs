using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;
using FinancialServices.Utils.Cache;
using Microsoft.Extensions.Logging;

namespace FinancialServices.Application.Security.UseCase
{
    public class AuthUserUseCase : IAuthUserUseCase
    {
        private readonly IRepository<UserEntity> userRepository;
        private readonly ILogger logger;
        private readonly IMapper mapper;
        

        public AuthUserUseCase(IRepository<UserEntity> userRepository, ILogger logger, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        
        [CachedMethod(minutes: 30)]
        public UserModel? AuthUser(string apiKey, string[] requiredRoles)
        {
            
            var user = userRepository.Query()
                .Where(p=>p.ApiKey == apiKey)
                .FirstOrDefault();

            if (user == null)
            {
                logger.LogWarning("Usuário não encontrado para a chave de API fornecida.");
                return default;
            }

            if (requiredRoles == null || !requiredRoles.Any())
            {
                logger.LogInformation("Usuário autenticado com sucesso.");
                return mapper.Map<UserModel>(user);
            }

            if (user.Roles == null || !user.Roles.Any())
            {
                logger.LogWarning("Usuário não possui nenhuma role atribuída.");
                return null;
            }

            var hasRequiredRole = user.Roles
                .Any(r => requiredRoles.Any(rr => rr.Equals(r, StringComparison.OrdinalIgnoreCase)));

            if (!hasRequiredRole)
            {
                logger.LogWarning("Usuário não possui roles necessárias para utilizar o endpoint.");
                return null;
            }

            logger.LogInformation("Usuário autenticado e autorizado com sucesso.");
            return mapper.Map<UserModel>(user);
        }
    }
}
