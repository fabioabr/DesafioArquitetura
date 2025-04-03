using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Model;
using FinancialServices.Utils.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinancialServices.Application.Financial.UseCase
{
    public class CreateConsolidatedReportsUseCase : ICreateConsolidatedReportsUseCase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IGetConsolidatedReportUseCase getConsolidatedReportUseCase;
        private readonly IRepository<TransactionGroupingEntity> transactionGroupingRepository;
        private readonly IRepository<TransactionEntity> transactionRepository;
        

        public CreateConsolidatedReportsUseCase(ILogger logger, IMapper mapper, IGetConsolidatedReportUseCase getConsolidatedReportUseCase, IRepository<TransactionGroupingEntity> transactionGroupingRepository, IRepository<TransactionEntity> transactionRepository, IOptions<ApplicationSettingsModel> settings)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.transactionGroupingRepository = transactionGroupingRepository;
            this.transactionRepository = transactionRepository;
            this.getConsolidatedReportUseCase = getConsolidatedReportUseCase;
            
        }         
        public GenericResponse CreateTransactionGroups(TimeZoneInfo[] timezonesToCache)
        {

            logger.LogInformation("CreateConsolidatedReportUseCase is running...");

            try
            {
                var lastReport = transactionGroupingRepository
                    .Query()
                    .OrderByDescending(x => x.Period)
                    .FirstOrDefault();

                var lastReportDate = DateTime.MinValue.ToUniversalTime();
                if (lastReport?.Period != null)
                    lastReportDate = lastReport.Period;

                if (transactionRepository.Query().Any())
                {
                    var groups = transactionRepository
                        .Query()
                        .Where(x => x.Timestamp >= lastReportDate.Date) // Todas as transações a partir da ultima data do ultimo report ou todas se for o 1º report                        
                        .GroupBy(x => new { Period = new DateTime(x.Timestamp.Year, x.Timestamp.Month, x.Timestamp.Day, x.Timestamp.Hour, 0, 0), x.Type })
                        .Select(x => new
                        {
                            Period = x.Key.Period,
                            Type = x.Key.Type,
                            Total = x.Sum(x => x.Amount),
                            Count = x.Count()
                        }).ToList();

                    var transactionGroups = groups
                        .GroupBy(x => new { x.Period, x.Type })
                        .Select(x => new TransactionGroupingEntity()
                        {
                            Id = Guid.NewGuid(),
                            Period = x.Key.Period,
                            TransactionType = x.Key.Type,
                            CreatedAt = DateTime.UtcNow,
                            TotalAmount = x.Sum(z => z.Total),
                            TransactionCount = x.Sum(z => z.Count),
                        }).ToList();


                    var maxPeriod = transactionGroups.Max(p => p.Period);
                    var minPeriod = transactionGroups.Min(p => p.Period);

                    // Lista os itens que ja existem no banco para descobrir o Id e substituir o registro
                    var existingPeriods = transactionGroupingRepository
                        .Query()
                        .Where(p => p.Period >= minPeriod && p.Period <= maxPeriod)
                        .Select(p => new { p.Period, p.TransactionType, p.Id })
                        .ToList();

                    // Reaproveita os IDs dos registros que ja existem no banco
                    transactionGroups
                        .GroupJoin(existingPeriods,
                            r => new { r.Period, r.TransactionType },
                            e => new { e.Period, e.TransactionType },
                            (r, e) => new { Report = r, Existing = e.FirstOrDefault() })
                        .Where(p => p.Existing != null)                        
                        .ToList()
                        .ForEach(p => p.Report.Id = p.Existing!.Id);

                    foreach (var transactionGroup in transactionGroups.OrderBy(p => p.Period).ThenBy(p => p.TransactionType))
                    {
                        // Cria o agrupamentro no banco de dados
                        logger.LogInformation("Creating report for Period = '{Period}' , TransactionType = '{TransactionType}' !", transactionGroup.Period, transactionGroup.TransactionType);
                        transactionGroupingRepository.InsertOrUpdate(transactionGroup);

                        // Obtem os Timezones configurados como preferidos no AppSettings/Envs                        
                        foreach (var timezone in timezonesToCache)
                        {
                            // Invalida o cache do agrupamento
                            logger.LogInformation("Revalidating cache for Period = '{Period}' , TransactionType = '{TransactionType}' !", transactionGroup.Period, transactionGroup.TransactionType);
                            var invalidatinResult = getConsolidatedReportUseCase.InvalidateTransactionGroupingCache(transactionGroup.Period, timezone, false);

                            // Loga o insucesso e vida que segue, pois o cache vai ser expirado quando vencer o tempo de vida
                            // e será recriado na requisição de consulta qdo necessário.
                            if (!invalidatinResult.Success)
                                logger.LogWarning("Was not possible to revalidate cache to Periodo = '{Period}' and Timezone = '{Timezone}', Message = '{message}'", transactionGroup.Period, transactionGroup.TransactionType, invalidatinResult.Message);

                        }
                         
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
                return new GenericResponse()
                    .WithMessage("Error creating Transaction Groups - {Message}", ex.Message)
                    .WithException(ex)
                    .WithFail();
            }
        }


    }
}
