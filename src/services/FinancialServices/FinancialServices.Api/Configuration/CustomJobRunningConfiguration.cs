using FinancialServices.Api.Jobs;
using FinancialServices.Domain.Model;
using Quartz;

namespace FinancialServices.Api.Configuration
{
    public static class CustomJobRunningConfiguration
    {
        public static WebApplicationBuilder AddCustomJobRunningConfiguration(this WebApplicationBuilder builder)
        {
            var settings = builder.Configuration.GetSection("CustomSettings").Get<ApplicationSettingsModel>()!;

            var useConsolidationReportJob = settings.UseConsolidationReportJob;

            if (useConsolidationReportJob) 
            {
                var cronConfig = settings.JobsSettings.CreateReportsJob.CronScheduleConfig;

                builder.Services.AddQuartz(q =>
                {

                    var consolidationJobKey = new JobKey("consolidation-job", "financial");

                    q.AddJob<CreateConsolidatedReportJob>(consolidationJobKey, j => j
                        .WithDescription("Job de consolidação de transações"));
                    /*
                    // Trigger para execução imediata
                    q.AddTrigger(t => t
                        .WithIdentity("consolidation-trigger-init", "financial")
                        .ForJob(consolidationJobKey)
                        .StartNow()
                        .WithDescription("Execução do Job na subida do Container"));
                    */
                    // Trigger para execução a cada 20 minutos
                    q.AddTrigger(t => t
                        .WithIdentity("consolidation-trigger-cron", "financial")
                        .ForJob(consolidationJobKey)
                        .WithCronSchedule(cronConfig)
                        .WithDescription("Execução a cada 20 minutos"));

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
