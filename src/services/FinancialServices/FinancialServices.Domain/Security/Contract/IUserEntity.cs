using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Security.Contract
{
    public interface IUserEntity
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string UserName { get; set; }
        IEnumerable<string> Roles { get; set; }
        string ApiKey { get; set; }
    }
}
