using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Enum;
using FinancialServices.Domain.Financial.Model;

namespace FinancialServices.Api.Utils.Seed
{
  
    public static class TransactionEntityGenerator
    {
        
        private static readonly Random _random = new Random();
         
        public static void GenerateDailyTransactions(DateTime date , TransactionEntityGeneratorRequest options, WebApplication app)
        {
            var transactionRepository = app.Services.GetRequiredService<IRepository<TransactionEntity>>();

            
            date = date.Date;

            if (options.PeakMomentTransactionCreationFactor + options.EasyMomentTransactionCreationFactor > 100)
                throw new ArgumentException("PeakMomentFactor + EasyMomentFactor must be less than or equal to 100");

            // Mais validações se necessário

            List<TransactionEntity> transactions = new List<TransactionEntity>();

            int transactionCountPerDay = options.TransactionCountAvgPerMinute * 60 * 24;

            // Obtem a quantidade total de transações para cada momento do dia
            var PeakMomentTransactionCount = transactionCountPerDay * options.PeakMomentTransactionCreationFactor / 100;
            var PeakBorderMomentTransactionCount = PeakMomentTransactionCount * options.PeakBorderMomentTransactionCreationFactor / 100 / 2;
            PeakMomentTransactionCount = PeakMomentTransactionCount - PeakBorderMomentTransactionCount;
            var easyMomentTransactionCount = transactionCountPerDay * options.EasyMomentTransactionCreationFactor / 100;
            var easyBorderMomentTransactionCount = easyMomentTransactionCount * options.EasyBorderMomentTransactionCreationFactor / 100 / 2;
            easyMomentTransactionCount = easyMomentTransactionCount - easyBorderMomentTransactionCount;
            var restMomentTransactionCount = transactionCountPerDay - PeakMomentTransactionCount - PeakBorderMomentTransactionCount - easyMomentTransactionCount - easyBorderMomentTransactionCount;

            // Descobre a minutos em cada tipo de periodo
            var peakMinuteCount = (int)(options.PeakMomentEnd - options.PeakMomentStart).TotalMinutes;
            var peakBorderMinuteCount = (int)(peakMinuteCount * options.PeakBorderDurationFactor / 100);
            peakMinuteCount = peakMinuteCount - peakBorderMinuteCount;

            var easyMinuteCount = (int)(options.EasyMomentEnd - options.EasyMomentStart).TotalMinutes;
            var easyBorderMinuteCount = (int)(peakMinuteCount * options.EasyBorderDurationFactor / 100);
            easyMinuteCount = easyMinuteCount - easyBorderMinuteCount;

            var restMomentMinuteCount = (int)peakMinuteCount + peakBorderMinuteCount + easyMinuteCount + easyBorderMinuteCount;

            // Contagem de transações por minuto em cada periodo
            var peakMomentTransactionCountPerMinute = (int)(PeakMomentTransactionCount / peakMinuteCount);
            var peakBorderMomentTransactionCountPerMinute = (int)(PeakBorderMomentTransactionCount / peakBorderMinuteCount);
            var easyMomentTransactionCountPerMinute = (int)(easyMomentTransactionCount / easyMinuteCount);
            var easyBorderMomentTransactionCountPerMinute = (int)(easyBorderMomentTransactionCount / easyBorderMinuteCount);
            var restMomentTransactionCountPerMinute = (int)(restMomentTransactionCount / restMomentMinuteCount);

            // Calcula o horário de cada momento
            var peakBorderMomentStart = date.Add(options.PeakMomentStart);
            var peakBorderMomentEnd = date.Add(options.PeakMomentEnd);
            var easyBorderMomentStart = date.Add(options.EasyMomentStart);
            var easyBorderMomentEnd = date.Add(options.EasyMomentEnd);

            var peakMomentStart = peakBorderMomentStart.AddMinutes(peakBorderMinuteCount);
            var peakMomentEnd = peakBorderMomentEnd.AddMinutes(easyBorderMinuteCount*-1);
            var easyMomentStart = easyBorderMomentStart.AddMinutes(easyBorderMinuteCount);
            var easyMomentEnd = easyBorderMomentEnd.AddMinutes(easyBorderMinuteCount * -1);


            // Descobre o fator de multiplicação para cada minuto do dia
            var transactioCountByMomentType = new Dictionary<MomentTypeEnum, double>();
            transactioCountByMomentType.Add(MomentTypeEnum.Peak, PeakMomentTransactionCount / peakMinuteCount);
            transactioCountByMomentType.Add(MomentTypeEnum.PeakBorder, PeakBorderMomentTransactionCount / peakBorderMinuteCount / 2);
            transactioCountByMomentType.Add(MomentTypeEnum.Easy, easyMomentTransactionCount / easyMinuteCount);
            transactioCountByMomentType.Add(MomentTypeEnum.EasyBorder, easyBorderMomentTransactionCount / easyBorderMinuteCount / 2);
            transactioCountByMomentType.Add(MomentTypeEnum.Default, restMomentTransactionCount / restMomentMinuteCount);
             
            // Neste momento temos a distribuição de transações por periodo do dia.
            // Agora precisamos distribuir as transações por hora dentro de cada momento do dia.
            var minutosDoDia = 24 * 60;
            var items = Enumerable.Range(0, minutosDoDia)
                .Select(m => date.AddMinutes(m))
                .Select(dt => new
                {
                    TimeStamp = dt,
                    MomentType = dt >= peakMomentStart && dt <= peakMomentEnd ? MomentTypeEnum.Peak :
                                 dt >= easyMomentStart && dt <= easyMomentEnd ? MomentTypeEnum.Easy :
                                 dt >= peakBorderMomentStart && dt <= peakBorderMomentEnd ? MomentTypeEnum.PeakBorder :
                                 dt >= easyBorderMomentStart && dt <= easyBorderMomentEnd ? MomentTypeEnum.EasyBorder :
                                 MomentTypeEnum.Default,
                })
                .Select(p => new
                {
                    p.TimeStamp,
                    p.MomentType,
                    TransactionCount = (int)(transactioCountByMomentType[p.MomentType] + (transactioCountByMomentType[p.MomentType] * (_random.Next(20) / 100.0) * (_random.Next(2) == 0 ? 1 : -1)))
                })
                .ToList();

          
            var records  = items.SelectMany(p =>
                Enumerable
                    .Range(0, p.TransactionCount)
                    .Select(i => new
                    {
                        Amount = _random.Next(1000, 50000) / 100.0M,
                        Timestamp = p.TimeStamp.AddSeconds(_random.Next(59)),
                        Type = _random.Next(100) <= 1 ? TransactionTypeEnum.Refund : _random.Next(2) == 0 ? TransactionTypeEnum.Credit : TransactionTypeEnum.Debit,
                        Description = $"Transaction #{_random.Next(100000)}",
                        DestinationAccount = $"AC{_random.Next(100000)}",
                        SourceAccount = $"AC{_random.Next(100000)}",
                        Id = Guid.NewGuid()
                    })
                    .Select(p => new TransactionEntity()
                    {
                        Amount = p.Amount * (p.Type == TransactionTypeEnum.Refund ? -1 : 1),
                        Timestamp = p.Timestamp,
                        Type = p.Type,
                        Description = p.Description,
                        DestinationAccount = p.DestinationAccount,
                        SourceAccount = p.SourceAccount,
                        Id = p.Id,
                        OriginalTransactionId = p.Type == TransactionTypeEnum.Refund ? Guid.NewGuid() : null
                    })
                     
            )
                .OrderBy(x => x.Timestamp)
                .ThenBy(x=> x.Type)
                .ToList();

          
            // Adiciona as novas transações
            transactionRepository.AddRangeAsync(records).Wait();
            
                      
        }
         
        public enum MomentTypeEnum
        {
            Peak,
            PeakBorder,
            Easy,
            EasyBorder,
            Default
        }

    }

    public class TransactionEntityGeneratorRequest
    {


        /// <summary>
        /// Media de transações por minuto no dia
        /// </summary>
        public int TransactionCountAvgPerMinute { get; set; }

        /// <summary>
        /// Horário de início do momento de pico (considerado hora e minuto)
        /// </summary>
        public TimeSpan PeakMomentStart { get; set; }

        /// <summary>
        /// Horário de término do momento de pico (considerado hora e minuto)
        /// </summary>
        public TimeSpan PeakMomentEnd { get; set; }

        /// <summary>
        /// Horário de início do momento calmo (considerado hora e minuto)
        /// </summary>
        public TimeSpan EasyMomentStart { get; set; }

        /// <summary>
        /// Horário de término do momento calmo (considerado hora e minuto)
        /// </summary>
        public TimeSpan EasyMomentEnd { get; set; }

        /// <summary>
        /// Fator de criação de transações no momento de pico em relação ao total de transações do dia
        /// </summary>
        public int PeakMomentTransactionCreationFactor { get; set; } // 50% do total de transações totais do dia

        /// <summary>
        /// Fator de criação de transações na borda do momento de pico em relação ao total de transações do momento de pico
        /// </summary>
        public int PeakBorderMomentTransactionCreationFactor { get; set; } // 30% do total de transações do momento de pico

        /// <summary>
        /// Fator de criação de transações no momento calmo em relação ao total de transações do dia
        /// </summary>
        public int EasyMomentTransactionCreationFactor { get; set; } // 15% do total de transações totais do dia

        /// <summary>
        /// Fator de criação de transações na borda momento calmo em relação ao total de transações do momento calmo
        /// </summary>
        public int EasyBorderMomentTransactionCreationFactor { get; set; } // 70% do total de transações do momento calmo

        /// <summary>
        /// Tempo de duração da borda do momento de pico
        /// </summary>
        public int PeakBorderDurationFactor { get; set; } // 10% do tempo do momento de pico divididos em 2 periodos (antes e depois)

        /// <summary>
        /// Tempo de duração da borda do momento calmo
        /// </summary>
        public int EasyBorderDurationFactor { get; set; } // 20% do tempo do momento calmo divididos em 2 periodos (antes e depois)

    }
     
}
