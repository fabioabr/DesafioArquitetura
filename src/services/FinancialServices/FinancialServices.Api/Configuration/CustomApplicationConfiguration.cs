using FinancialServices.Api.Model;
using FinancialServices.Application.Security.UseCase;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Infrastructure.Data.Concrete;
using FinancialServices.Infrastructure.Data.Concrete.Factory;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Data.Contract.Repository;
using FinancialServices.Infrastructure.Enum;
using FinancialServices.Utils.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;
using System.Reflection.PortableExecutable;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
