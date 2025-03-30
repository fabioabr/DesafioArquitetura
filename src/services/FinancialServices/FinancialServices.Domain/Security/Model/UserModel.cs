using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Domain.Security.Model
{
    public class UserModel
    {        
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string UserName { get; set; }
        public required IEnumerable<string> Roles { get; set; } = [];
        public required string ApiKey { get; set; }



    }
}
