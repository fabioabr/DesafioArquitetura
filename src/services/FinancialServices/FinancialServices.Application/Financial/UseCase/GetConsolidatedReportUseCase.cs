using AutoMapper;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Utils.Cache;
using FinancialServices.Utils.Shared;
using System.Reflection;

namespace FinancialServices.Application.Financial.UseCase
{
    public class GetConsolidatedReportUseCase : IGetConsolidatedReportUseCase
    {
        private readonly IMapper mapper;
        private readonly IRepository<ConsolidatedReportEntity> consolidatedReportModelRepository;

        public GetConsolidatedReportUseCase(IMapper mapper, IRepository<ConsolidatedReportEntity> consolidatedReportModelRepository)
        {
            this.consolidatedReportModelRepository = consolidatedReportModelRepository;
            this.mapper = mapper;
        }

        [CachedMethod(minutes: 20)]
        public GenericResponse<ConsolidatedReportModel> GetConsolidatedReport(DateTime date, int timezoneOffset)
        {

            if (date.ToUniversalTime().Date > DateTime.UtcNow.Date)
            {
                // Não permite pesquisar datas futuras
                return new GenericResponse<ConsolidatedReportModel>()
                    .WithMessage("Invalid Date")
                    .WithException(new InvalidDataException("Invalid Date"))
                    .WithFail();
            }

            // Tenta obter o report da data informada no parametro
            var report = consolidatedReportModelRepository
                .Query()
                .Where(x => x.Date == date.Date && x.TimezoneOffset == timezoneOffset)
                .FirstOrDefault();
             
            if (report == null)
            {

                return new GenericResponse<ConsolidatedReportModel>()
                    .WithMessage("Report Not Found")
                    .WithException(new KeyNotFoundException("Report Not Found"))
                    .WithFail();

            }

            return new GenericResponse<ConsolidatedReportModel>()                  
                  .WithData(mapper.Map<ConsolidatedReportModel>(report))
                  .WithSuccess();
             
        }

        
        public GenericResponse InvalidateReportCache(DateTime date, int timezoneOffset, bool revalidate)
        {
            Type typeOfMe = this.GetType();

            MethodInfo methodInfo = typeOfMe.GetMethod("GetConsolidatedReport", new Type[] { typeof(DateTime), typeof(int) });

            if (methodInfo != null)
            {
                try
                {
                    var key = CacheAspect.GenerateCacheKey(methodInfo, new object[] { date, timezoneOffset });
                    MethodBase methodBase = methodInfo;
                    CacheAspect.Cache.Invalidate(key);

                    if (revalidate)
                        GetConsolidatedReport(date, timezoneOffset);

                }
                catch (Exception ex)
                {
                    return new GenericResponse()
                        .WithMessage(ex.Message)
                        .WithException(ex)
                        .WithFail();

                }

            }

            return new GenericResponse()
                .WithSuccess();


        }
    }
}
