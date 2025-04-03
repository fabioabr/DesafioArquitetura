using FinancialServices.Domain.Core.Attributes;
using FinancialServices.Domain.Core.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialServices.Domain.Security.Entity
{

    [EntitySetName("Users")]
    public class UserEntity : IEntity
    {
        
        public  Guid Id { get; set; } = Guid.NewGuid();
        public  string Name { get; set; } = string.Empty;
        public  string UserName { get; set; } = string.Empty;
        public  IEnumerable<string> Roles { get; set; } = [];
        public string ApiKey { get; set; } = string.Empty;     
    }
}
