using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Data.Attributes
{
    
    [AttributeUsage(AttributeTargets.Interface)]
    public class EntitySetNameAttribute : Attribute
    {       
        public EntitySetNameAttribute(string name)
        {
            Name = name;
        }
         
        public string Name { get; }
         
    }
}
