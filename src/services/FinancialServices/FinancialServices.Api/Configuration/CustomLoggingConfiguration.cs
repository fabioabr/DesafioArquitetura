using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;

namespace FinancialServices.Api.Configuration
{
    public static class CustomLoggerConfiguration
    {
        public static WebApplicationBuilder AddCustomLoggingConfiguration(this WebApplicationBuilder builder)
        {

            var grafanaLokiUrl = builder.Configuration["CustomSettings:Observability:GrafanaLokiUrl"] ?? throw new System.Exception("CustomSettings:Observability:GrafanaLokiUrl is not set");
            var applicationName = builder.Environment.ApplicationName;
            var environment = builder.Environment.EnvironmentName;
             
            
            // Inicialmente cria um log com o Serilog
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProperty("Environment", environment)
                .Enrich.FromLogContext()                
                .WriteTo.Console()
                .WriteTo.GrafanaLoki(grafanaLokiUrl).Filter.ByExcluding(
                    Matching.WithProperty(
                        "RequestPath", (string p) => 
                            p.EndsWith("/health") || p.EndsWith("/metrics") || p.Contains("/loki/api/v1/push")
                )).CreateLogger();


            // Agora Cria um ILogger para não ficarmos dependentes do ILogger do Serilog
            var logger = LoggerFactory.Create(delegate (ILoggingBuilder logBuilder)
            {
                logBuilder
                    .SetMinimumLevel(builder.Environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information)
                    .AddSerilog(Log.Logger, dispose: true);                    

            }).CreateLogger(applicationName);

            builder.Services.AddSingleton(logger);

            return builder;
        }


    }
}
