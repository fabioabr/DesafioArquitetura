using FinancialServices.Api.Model;
using FinancialServices.Infrastructure.Enum;
using System.Reflection;

namespace FinancialServices.Api.Configuration
{
    public static class CustomMappingConfiguration
    {

        public static WebApplicationBuilder AddCustomMappingConfiguration(this WebApplicationBuilder builder)
        {            
            var assemblyName = "FinancialServices.Application";
            var assembly = Assembly.Load(assemblyName);
            builder.Services.AddAutoMapper(assembly);
            return builder;

        }
    }
}
