using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Model;
using Microsoft.Extensions.Options;
using Quartz;
using System;

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

            var timezones = settings.Value.JobsSettings.CreateReportsJob.Timezones
                .Select(timezoneId => TimeZoneInfo.FindSystemTimeZoneById(timezoneId))
                .ToArray();

            var result = createConsolidatedReportsUseCase.CreateTransactionGroups(timezones);

            if (result.Success)
                logger.LogInformation("CreateConsolidatedReportJob completed successfully");
            else
                logger.LogError(result.Exception, "CreateConsolidatedReportJob completed with one or more errors");

            return Task.CompletedTask;
        }
    }
}
