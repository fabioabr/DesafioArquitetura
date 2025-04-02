using FinancialServices.Api.Jobs;
using Quartz;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;
using System.Globalization;

namespace FinancialServices.Api.Configuration
{
    public static class CustomJobRunningConfiguration
    {
        public static WebApplicationBuilder AddCustomJobRunningConfiguration(this WebApplicationBuilder builder)
        {
          
            var useConsolidationReportJob = (Environment.GetEnvironmentVariable("CustomSettings__UseConsolidationReportJob") ?? "true") == "true";

            if (useConsolidationReportJob) 
            {
                var cronConfig = Environment.GetEnvironmentVariable("CustomSettings__Jobs__CronScheduleConfig") ?? "0 */20 * * * ?"; // 20 / 20 minutos como padrão

                builder.Services.AddQuartz(q =>
                {

                    var consolidationJobKey = new JobKey("consolidation-job", "financial");

                    q.AddJob<CreateConsolidatedReportJob>(consolidationJobKey, j => j
                        .WithDescription("Job de consolidação de transações"));

                    q.AddTrigger(t => t
                        .WithIdentity("consolidation-trigger", "financial")
                        .ForJob(consolidationJobKey)
                        .StartNow()
                        .WithCronSchedule(cronConfig) // configração para executar o job baseado nas configurações
                        .WithDescription("Trigger que executa o job de consolidação das transações"));

                });

                builder.Services.AddQuartzHostedService(options =>
                {
                    // when shutting down we want jobs to complete gracefully
                    options.WaitForJobsToComplete = true;
                });

            }
          
            return builder;
        }

    }

}
