
using FinancialServices.Api.Model;
using FinancialServices.Domain.Financial.Contract;
using Microsoft.Extensions.Options;
using Quartz;

namespace FinancialServices.Api.Jobs
{
    public class CreateConsolidatedReportJob : IJob
    {
        private readonly ILogger logger;
        private readonly ICreateConsolidatedReportsUseCase createConsolidatedReportsUseCase;
        private readonly IOptions<ApplicationSettingsModel> settings;
        public CreateConsolidatedReportJob(ILogger logger, ICreateConsolidatedReportsUseCase createConsolidatedReportsUseCase, IOptions<ApplicationSettingsModel> settings)
        {
            this.logger = logger;
            this.createConsolidatedReportsUseCase = createConsolidatedReportsUseCase;
            this.settings = settings;
        }

        public Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("CreateConsolidatedReportJob is running...");

            var timezoneOffsets = settings.Value.JobsSettings.CreateReportsJob.TimezoneOffsets.Split(",");

            var hasErrors = false;

            foreach(var tz in timezoneOffsets)
            {
                var result = createConsolidatedReportsUseCase.CreateConsolidatedReport(0);
                hasErrors = hasErrors || !result.Success;
            }

            if (!hasErrors)
                logger.LogInformation("CreateConsolidatedReportJob completed successfully");
            else
                logger.LogError("CreateConsolidatedReportJob completed with one or more errors");

            return Task.CompletedTask;
        }
    }
}
