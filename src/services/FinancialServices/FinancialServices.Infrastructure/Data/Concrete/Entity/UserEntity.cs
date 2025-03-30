using FinancialServices.Infrastructure.Data.Attributes;
using FinancialServices.Infrastructure.Data.Contract.Entity;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialServices.Infrastructure.Data.Concrete.Entity
{

    [Table("users")]
    public class UserEntity : IEntity
    {
        
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string UserName { get; set; }
        public required IEnumerable<string> Roles { get; set; } = [];
        public required string ApiKey { get; set; }
    }
}
