﻿using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Security.Model;
using FinancialServices.Utils.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Financial.Contract
{
    public interface IGetConsolidatedReportUseCase
    {
        GenericResponse<ConsolidatedReportModel> GetConsolidatedReport (DateTime date, int timezoneOffset);

        GenericResponse InvalidateReportCache(DateTime date, int timezoneOffset, bool revalidate);
    }
}
