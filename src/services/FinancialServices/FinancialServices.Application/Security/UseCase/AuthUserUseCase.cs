using AutoMapper;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Model;
using FinancialServices.Infrastructure.Data.Contract.Repository;
using FinancialServices.Infrastructure.Utils.Cache;
using Microsoft.Extensions.Logging;

namespace FinancialServices.Application.Security.UseCase
{
    public class AuthUserUseCase : IAuthUserUseCase
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger logger;
        private readonly IMapper mapper;
        

        public AuthUserUseCase(IUserRepository userRepository, ILogger logger, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        
        [CachedMethod(minutes: 30)]
        public async Task<UserModel?> AuthUserAsync(string apiKey, string[] requiredRoles)
        {
            
            var user = await userRepository.GetByApiKeyAsync(apiKey);

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
