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
        public  Guid Id { get; set; }  = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public    IEnumerable<string> Roles { get; set; } = [];
        public string ApiKey { get; set; } = string.Empty;



    }
}
