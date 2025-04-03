using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Utils.Cache
{
    public interface IInMemoryCacheService
    {
        void Set(string key, object value, TimeSpan ttl);
        bool TryGet(string key, out object value);
        void Invalidate(string key);
        void InvalidateByPrefix(string prefix);
        void InvalidateByArgument(string prefix, int argumentIndex, object argumentValue);
    }

}
