using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeOlho.ETL
{
    public interface IStep<T>  where T : class
    {
        IStep<TOut> Transform<TOut>(Func<StepValue<T>, TOut> transform) where TOut : class;
        IStep<TOut> TransformAsync<TOut>(Func<StepValue<T>, Task<TOut>> transform) where TOut : class;
        IEnumerable<StepValue<TOut>> TransformToList<TOut>(Func<StepValue<T>, IEnumerable<TOut>> transform) where TOut : class;
        IStep<TOut> Extract<TOut>(Func<StepValue<T>, ISource<TOut>> extract) where TOut : class;
        Task<StepValue<T>> Load(Func<IDestination> destination);
        Task<StepValue<T>> Load();
        Task<StepValue<T>> Execute(); 
        
    }
}