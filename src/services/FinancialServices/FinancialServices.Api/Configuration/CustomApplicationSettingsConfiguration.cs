using FinancialServices.Api.Model;

namespace FinancialServices.Api.Configuration
{
    public static class CustomApplicationSettings
    {
        public static WebApplicationBuilder AddCustomApplicationSettingsConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddOptions<ApplicationSettingsModel>()
                .Bind(builder.Configuration.GetSection("CustomSettings"))
                .ValidateDataAnnotations(); 

            return builder;
             
        }
    }
}
