using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Common
{
    public static class LinqExtensions
    {
        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> projectionAsync)
        {
            var result = new List<TResult>();

            foreach (var item in source)
            {
                var projection = await projectionAsync(item);

                result.Add(projection);
            }

            return result;
        }

        public static async Task<IEnumerable<T>> Memoize<T>(this Task<IEnumerable<T>> source)
        {
            var enumerable = await source;

            return enumerable.ToArray();
        }
    }
}
