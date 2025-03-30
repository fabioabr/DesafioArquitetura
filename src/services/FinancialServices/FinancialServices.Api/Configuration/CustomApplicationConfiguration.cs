using FinancialServices.Application.Security.UseCase;
using FinancialServices.Domain.Security.Contract;

namespace FinancialServices.Api.Configuration
{
    public static class CustomApplicationConfiguration
    {
        public static WebApplicationBuilder AddCustomApplicationConfiguration(this WebApplicationBuilder builder)
        {
          
            builder.Services.AddSingleton<IAuthUserUseCase, AuthUserUseCase>();

            return builder;
                
        }

    }
}
