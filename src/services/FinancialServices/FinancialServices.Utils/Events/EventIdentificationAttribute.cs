using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Utils
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventIdentificationAttribute() : Attribute
    {
        public required string Topic { get; set; }
        public string Type { get; set; } = "topic";        
    }
}
