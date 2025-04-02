using AspectInjector.Broker;
using System.Reflection;

namespace FinancialServices.Utils.Cache
{
    [Aspect(Scope.Global)]   
    public class CacheAspect
    {
         
        public static readonly IInMemoryCacheService Cache = new InMemoryCacheService();

        [Advice(Kind.Around, Targets = Target.Method)]
        public object HandleMethod(
            [Argument(Source.Target)] Func<object[], object> method,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Metadata)] MethodBase methodInfo)
        {
            var attr = methodInfo.GetCustomAttribute<CachedMethodAttribute>()!;
            var key = CacheAspect.GenerateCacheKey(methodInfo, args);

            if (Cache.TryGet(key, out var cached))
                return cached;

            var result = method(args);

            Cache.Set(key, result, attr.Duration);
            return result;
        }

        public static string GenerateCacheKey(MethodBase methodInfo, object[] args)
        {
            return $"{methodInfo!.DeclaringType!.FullName}.{methodInfo.Name}:{string.Join("_", args.Select(a => a?.ToString()))}";
        }
    }
}
