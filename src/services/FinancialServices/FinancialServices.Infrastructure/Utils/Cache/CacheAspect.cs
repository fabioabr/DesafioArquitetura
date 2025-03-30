using AspectInjector.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Utils.Cache
{
    [Aspect(Scope.Global)]   
    public class CacheAspect
    {
        private static readonly IInMemoryCacheService _cache = new InMemoryCacheService();

        [Advice(Kind.Around, Targets = Target.Method)]
        public object HandleMethod(
            [Argument(Source.Target)] Func<object[], object> method,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Metadata)] MethodBase methodInfo)
        {
            var attr = methodInfo.GetCustomAttribute<CachedMethodAttribute>()!;
            var key = $"{methodInfo!.DeclaringType!.FullName}.{methodInfo.Name}:{string.Join("_", args.Select(a => a?.ToString()))}";

            if (_cache.TryGet(key, out var cached))
                return cached;

            var result = method(args);

            _cache.Set(key, result, attr.Duration);
            return result;
        }
    }
}
