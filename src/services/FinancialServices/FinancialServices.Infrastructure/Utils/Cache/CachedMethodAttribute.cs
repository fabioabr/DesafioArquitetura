using AspectInjector.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Utils.Cache
{
    [AttributeUsage(AttributeTargets.Method)]
    [Injection(typeof(CacheAspect))] 
    public class CachedMethodAttribute : Attribute
    {
        public TimeSpan Duration { get; }

        public CachedMethodAttribute(int hours = 0, int minutes = 0, int seconds = 0)
        {
            var totalSeconds = hours * 3600 + minutes * 60 + seconds;
            this.Duration = TimeSpan.FromSeconds(totalSeconds);
        }
    }
}
