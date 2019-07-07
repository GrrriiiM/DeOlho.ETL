using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeOlho.ETL
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<StepValue<TOut>> Extract<TIn, TOut>(this IEnumerable<StepValue<TIn>> value, Func<StepValue<TIn>, ISource<TOut>> extract) where TIn : class where TOut : class
        {
            return Enumerable.Select(value, _ => new StepValue<TOut>(extract(_).Execute().Result , _));
        }
        public static IEnumerable<StepValue<TOut>> Transform<TIn, TOut>(this IEnumerable<StepValue<TIn>> value, Func<StepValue<TIn>, TOut> selector) where TIn : class where TOut : class
        {
            return Enumerable.Select(value, _ => new StepValue<TOut>(selector(_), _));
        }

        public static IEnumerable<StepValue<TOut>> TransformToList<TIn, TOut>(this IEnumerable<StepValue<TIn>> value, Func<StepValue<TIn>, IEnumerable<TOut>> transform) where TOut : class
        {
            return Enumerable.SelectMany(value, _ => transform(_).Select(_1 => new StepValue<TOut>(_1, _)));
        }

        public async static Task<IEnumerable<StepValue<T>>> Load<T>(this IEnumerable<StepValue<T>> value, Func<IDestination> destination) where T : class
        {
            return await destination().Execute(value);
        }

        public async static Task<IEnumerable<StepValue<T>>> Execute<T>(this IEnumerable<StepValue<T>> value) where T : class
        {
            await Task.CompletedTask;
            return value.ToList();
        }
    }
}