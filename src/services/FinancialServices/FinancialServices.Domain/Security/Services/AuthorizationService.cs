using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Utils.Shared;

namespace FinancialServices.Domain.Security.Services
{
    public class AuthorizationService : IAuthorizationService
    {

        private readonly IRepository<UserEntity> userRepository;
        public AuthorizationService(IRepository<UserEntity> userRepository)
        {
            this.userRepository = userRepository;
        }

        public GenericResponse AuthorizeUserByApiKey(string apiKey, string[] requiredRoles)
        {
            var response = new GenericResponse();

            var user = userRepository.Query()
                .Where(p => p.ApiKey == apiKey)
                .FirstOrDefault();

            if (user == null)
                response
                    .WithFail()
                    .WithMessage("User not found");

            if (requiredRoles == null || !requiredRoles.Any())
                return response
                    .WithSuccess()
                    .WithMessage("User Authenticated");

            if (user!.Roles == null || !user.Roles.Any())
                return response
                    .WithFail()
                    .WithMessage("User has no roles");

            var hasRequiredRole = user.Roles
                .Any(r => requiredRoles.Any(rr => rr.Equals(r, StringComparison.OrdinalIgnoreCase)));

            if (!hasRequiredRole)
                return response
                   .WithFail()
                   .WithMessage("User has no valid roles to use this service");

            return response
                .WithSuccess()
                .WithMessage("User Authenticated");



        }
    }
}
