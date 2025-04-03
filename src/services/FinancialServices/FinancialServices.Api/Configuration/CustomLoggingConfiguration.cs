using FinancialServices.Domain.Model;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;

namespace FinancialServices.Api.Configuration
{
    public static class CustomLoggerConfiguration
    {
        public static WebApplicationBuilder AddCustomLoggingConfiguration(this WebApplicationBuilder builder)
        {
            var settings = builder.Configuration.GetSection("CustomSettings").Get<ApplicationSettingsModel>()!;
            var applicationName = builder.Environment.ApplicationName;
            var environment = builder.Environment.EnvironmentName;

            var grafanaLokiUrl = settings.ObservabilitySettings.GrafanaLokiUrl;

             
            // Inicialmente cria um log com o Serilog
            var logConfig = new LoggerConfiguration()
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProperty("Environment", environment)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            // Se tem a URL do Loki então adiciona como destino dos logs
            
            // Uma possibilidade é usar o coletor Promtail no cluster ao inves de enviar o log direto da aplicação
            // é uma questão de tradeoffs 

            if (!string.IsNullOrEmpty(grafanaLokiUrl))
                logConfig.WriteTo.GrafanaLoki(grafanaLokiUrl).Filter.ByExcluding(
                    Matching.WithProperty(
                        "RequestPath", (string p) =>
                            p.EndsWith("/health") || p.EndsWith("/metrics") || p.Contains("/loki/api/v1/push")
                ));

            // Cria o log no formato Serilog
            Log.Logger = logConfig.CreateLogger();

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
