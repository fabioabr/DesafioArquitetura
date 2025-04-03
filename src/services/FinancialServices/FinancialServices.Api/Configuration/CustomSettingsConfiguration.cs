using FinancialServices.Domain.Model;
using FinancialServices.Infrastructure.Enums;

namespace FinancialServices.Api.Configuration
{
    public static class CustomSettingsConfiguration
    {
        public static WebApplicationBuilder AddCustomApplicationSettingsConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddOptions<ApplicationSettingsModel>()
                .Bind(builder.Configuration.GetSection("CustomSettings"))
                .ValidateDataAnnotations()
                .Validate(x => Enum.IsDefined(typeof(DatabaseTypeEnum), x.DatabaseToUse), "Invalid database provider specified");
            ; 

            return builder;
             
        }
    }
}
