using FinancialServices.Application.Financial.UseCase;
using FinancialServices.Application.Security.UseCase;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Service;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Services;

namespace FinancialServices.Api.Configuration
{
    public static class CustomApplicationConfiguration
    {
        public static WebApplicationBuilder AddCustomApplicationConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IAuthUserUseCase, AuthUserUseCase>();
            builder.Services.AddSingleton<ICreateTransactionUseCase, CreateTransactionUseCase>();
            builder.Services.AddSingleton<IGetConsolidatedReportUseCase, GetConsolidatedReportUseCase>();

            builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();                         
            builder.Services.AddSingleton<ITransactionValidationService, TransactionValidationService>();
             
            return builder;
                
        }

    }
}
