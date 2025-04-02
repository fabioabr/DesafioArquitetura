using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;
using FinancialServices.Utils.Cache;
using FinancialServices.Utils.Shared;
using Microsoft.Extensions.Logging;

namespace FinancialServices.Application.Security.UseCase
{
    public class AuthUserUseCase : IAuthUserUseCase
    {
        private readonly IRepository<UserEntity> userRepository;
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IAuthenticationService authenticationService;
        private readonly IAuthorizationService authorizationService;
         
        public AuthUserUseCase(
            IRepository<UserEntity> userRepository,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ILogger logger, 
            IMapper mapper)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.authenticationService = authenticationService;
            this.authorizationService = authorizationService;
        }


        [CachedMethod(minutes:10)]
        public GenericResponse<UserModel?> Execute(string apiKey, string[] requiredRoles)
        {
            var response = new GenericResponse<UserModel?>();
            
            // 1. Authenticate user by apiKey    
            if (!authenticationService.AuthenticateByApiKey(apiKey))
            {                                   
                return response
                    .WithMessage("Não foi possivel autenticar o usuário pela API KEY informada.")
                    .WithFail();
            }

            // 2. Check user roles
            var authorizationResult = authorizationService.AuthorizeUserByApiKey(apiKey, requiredRoles);

            if (!authorizationResult.Success)
            {
                return response
                    .WithMessage(authorizationResult.Message)
                    .WithFail(); 
            }

            var userEntity = userRepository
                .Query()
                .Where(p=>p.ApiKey == apiKey)
                .FirstOrDefault();

            return response
                .WithData(mapper.Map<UserModel>(userEntity))
                .WithMessage("User Authenticated!")
                .WithSuccess();
             
        }
    }
}
