using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Enum;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Utils.Cache;
using FinancialServices.Utils.Shared;
using Serilog.Core;
using System;
using System.Reflection;

namespace FinancialServices.Application.Financial.UseCase
{
    public class GetConsolidatedReportUseCase : IGetConsolidatedReportUseCase
    {
        private readonly IMapper mapper;
        private readonly IRepository<TransactionGroupingEntity> transactionGroupingRepository;

        public GetConsolidatedReportUseCase(IMapper mapper, IRepository<TransactionGroupingEntity> transactionGroupingRepository)
        {
            this.transactionGroupingRepository = transactionGroupingRepository;
            this.mapper = mapper;
        }

        [CachedMethod(minutes: 20)]
        public GenericResponse<List<TransactionGroupingModel>?> GetConsolidatedReport(DateTime date, TimeZoneInfo timezone)
        {

            var response = new GenericResponse<List<TransactionGroupingModel>?>();

            // Não permite pesquisar datas futuras
            if (date.ToUniversalTime().Date > DateTime.UtcNow.Date)
            {
                return response
                    .WithMessage("Invalid Date")
                    .WithException(new InvalidDataException("Invalid Date"))
                    .WithFail();
            }

            var userDate = date.AddHours(timezone.BaseUtcOffset.Hours * -1);                
            var minUtcTimestamp = userDate;
            var maxUtcTimestamp = userDate.AddDays(1).AddMinutes(-1);

            // Tenta obter os Reports de um determinado dia informado no parametro
            var reports = transactionGroupingRepository
                .Query()
                .Where(x => x.Period >= minUtcTimestamp && x.Period <= maxUtcTimestamp)
                .OrderBy(x => x.Period)
                .ThenBy(x => x.TransactionType)
                .ToList();

            // Reescreve a data baseado no timezoneOffset
            reports.ForEach(p =>
                p.Period = TimeZoneInfo.ConvertTime(p.Period, timezone) // Converte o UTC para o timezone do usuario
            );

            // retorna os agrupamentos consolidados
            return response
                  .WithData(mapper.Map<List<TransactionGroupingModel>>(reports))
                  .WithSuccess();

        }


        public GenericResponse InvalidateTransactionGroupingCache(DateTime date, TimeZoneInfo timezone, bool eraseAllRecordsOfDay)
        {
            MethodInfo methodInfo = this
                .GetType()!
                .GetMethod("GetConsolidatedReport", new Type[] { typeof(DateTime), typeof(TimeZoneInfo) })!;

            try
            {
                var baseDate = eraseAllRecordsOfDay ? new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0) : date;
                var itemCount = eraseAllRecordsOfDay ? 24 : 1;

                Enumerable
                    .Range(0, itemCount)
                    .ToList()
                    .ForEach(p =>
                    {
                        var key = CacheAspect.GenerateCacheKey(methodInfo, [baseDate.AddHours(p), timezone]);

                        // Invalida o Cache
                        CacheAspect.Cache.Invalidate(key);

                    });

            }
            catch (Exception ex)
            {
                return new GenericResponse()
                    .WithMessage(ex.Message)
                    .WithException(ex)
                    .WithFail();

            }

            return new GenericResponse()
                .WithSuccess();


        }
    }
}
