using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Utils.Cache;
using FinancialServices.Utils.Shared;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Application.Financial.UseCase
{
    public class CreateConsolidatedReportsUseCase : ICreateConsolidatedReportsUseCase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IGetConsolidatedReportUseCase getConsolidatedReportUseCase;
        private readonly IRepository<ConsolidatedReportEntity> consolidatedReportsRepository;
        private readonly IRepository<TransactionEntity> transactionRepository;

        public CreateConsolidatedReportsUseCase(ILogger logger, IMapper mapper, IGetConsolidatedReportUseCase getConsolidatedReportUseCase, IRepository<ConsolidatedReportEntity> consolidatedReportsRepository, IRepository<TransactionEntity> transactionRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.consolidatedReportsRepository = consolidatedReportsRepository;
            this.transactionRepository = transactionRepository;
            this.getConsolidatedReportUseCase = getConsolidatedReportUseCase;
        }


        public GenericResponse CreateConsolidatedReport(int timezoneOffset = 0)
        {

            logger.LogInformation("CreateConsolidatedReportUseCase is running...");

            try
            {
                var lastReport = consolidatedReportsRepository
                    .Query()
                    .Where(x => x.TimezoneOffset == timezoneOffset)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();

                DateTime lastReportDateUtc = DateTime.UtcNow;
                if (lastReport != null)
                    lastReportDateUtc = lastReport.Date.ToUniversalTime();


                if (transactionRepository.Query().Any())
                {
                    var groups = transactionRepository
                        .Query()
                        .Where(x => x.Timestamp >= lastReportDateUtc)
                        .ToList()
                        .Select(x => new
                        {
                            LocalTimestamp = TimeZoneInfo.ConvertTimeFromUtc(x.Timestamp, TimeZoneInfo.CreateCustomTimeZone("CustomTimeZone", TimeSpan.FromHours(timezoneOffset), "CustomTimeZone", "CustomTimeZone")),
                            Type = x.Type,
                            Amount = x.Amount
                        })
                        .GroupBy(x => new { x.LocalTimestamp.Date, x.LocalTimestamp.Hour, x.Type })
                        .Select(x => new
                        {
                            Timestamp = x.Key.Date,
                            Hour = x.Key.Hour,
                            Type = x.Key.Type,
                            Total = x.Sum(x => x.Amount)
                        }).ToList();

                    var reports = groups
                        .GroupBy(x => x.Timestamp.Date)
                        .Select(x => new ConsolidatedReportModel()
                        {
                            Id = Guid.NewGuid(),
                            Date = x.Key,
                            TimezoneOffset = timezoneOffset,
                            Items = x.Select(z => new ConsolidatedReportItemModel()
                            {
                                Start = new DateTime(z.Timestamp.Year, z.Timestamp.Month, z.Timestamp.Day, z.Hour, 0, 0),
                                End = new DateTime(z.Timestamp.Year, z.Timestamp.Month, z.Timestamp.Day, z.Hour, 59, 59),
                                TransactionType = z.Type,
                                TotalAmount = x.Sum(n => n.Total),
                                TransactionCount = x.Count(),
                            }).ToList()
                        }).ToList();

                    foreach (var report in reports)
                    {

                        logger.LogInformation("Creating reporting to Date = '{ReportDate}' adn Timezone = '{Timezone}'", report.Date, report.TimezoneOffset);

                        var existingReport = consolidatedReportsRepository
                            .Query()
                            .Where(p => p.Date == report.Date && p.TimezoneOffset == report.TimezoneOffset)
                            .FirstOrDefault()
                            ;

                        // Utiliza o id do report q ja existe para substituir o registro caso ja exista no banco
                        report.Id = existingReport?.Id ?? report.Id;
                        var entity = mapper.Map<ConsolidatedReportEntity>(report);

                        consolidatedReportsRepository.InsertOrUpdate(entity);
                        logger.LogInformation("Report from '{ReportDate}' created!", entity.Date);


                        logger.LogInformation("Revalidating Cache to Date = '{ReportDate}' and Timezone = '{Timezone}'", report.Date, report.TimezoneOffset);

                        var invalidatinResult = getConsolidatedReportUseCase.InvalidateReportCache(report.Date, report.TimezoneOffset, true);

                        if (invalidatinResult.Success)
                            logger.LogInformation("Report cache created to Date = '{ReportDate}' and Timezone = '{Timezone}'", report.Date, report.TimezoneOffset);
                        else
                            logger.LogWarning("Was not possible to invalidate cache to Date = '{ReportDate}' and Timezone = '{Timezone}', Message = '{message}'", report.Date, report.TimezoneOffset, invalidatinResult.Message);

                    }

                    return new GenericResponse()
                        .WithSuccess();

                }
                else
                {

                    return new GenericResponse()
                        .WithMessage("Has no report to be created")
                        .WithSuccess();

                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error creating reports to Timezone Offset = '{TimezoneOffset}' - Message {Message}", timezoneOffset, ex.Message);

                return new GenericResponse()
                    .WithMessage($"Error creating reports to Timezone Offset = '{timezoneOffset}' - Message {ex.Message}")
                    .WithException(ex)
                    .WithFail();
            }
        }


    }
}
