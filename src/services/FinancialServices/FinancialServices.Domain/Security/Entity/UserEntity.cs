using FinancialServices.Domain.Core.Attributes;
using FinancialServices.Domain.Core.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialServices.Domain.Security.Entity
{

    [EntitySetName("Users")]
    public class UserEntity : IEntity
    {
        
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string UserName { get; set; }
        public required IEnumerable<string> Roles { get; set; } = [];
        public required string ApiKey { get; set; }
    }
}
