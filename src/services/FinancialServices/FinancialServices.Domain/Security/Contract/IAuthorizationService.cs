using FinancialServices.Domain.Security.Model;
using FinancialServices.Utils.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Security.Contract
{
    public interface IAuthorizationService
    {
        GenericResponse AuthorizeUserByApiKey(string apiKey, string[] requiredRoles);

    }
}
