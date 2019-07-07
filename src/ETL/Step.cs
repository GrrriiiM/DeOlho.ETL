using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeOlho.ETL.Destinations;

namespace DeOlho.ETL
{
    public abstract class Step<T> : IStep<T> where T : class
    {

        public IStep<TOut> Transform<TOut>(Func<StepValue<T>, TOut> transform) where TOut : class
        {
            return new StepTransform<T, TOut>(this, async (_) =>
            { 
                return await Task.Run<TOut>(() => transform(_));
            });
        }

        public IStep<TOut> TransformAsync<TOut>(Func<StepValue<T>, Task<TOut>> transform) where TOut : class
        {
            return new StepTransform<T, TOut>(this, transform);
        }



        public IEnumerable<StepValue<TOut>> TransformToList<TOut>(Func<StepValue<T>, IEnumerable<TOut>> transform) where TOut : class
        {
            return new StepToCollectionTransform<T, TOut>(this, transform);
        }

        public IStep<TOut> Extract<TOut>(Func<StepValue<T>, ISource<TOut>> extract) where TOut : class
        {
            return new StepTransform<T, TOut>(this, async (_) => await extract(_).Execute());
        }

        public async Task<StepValue<T>> Load(Func<IDestination> destination)
        {
            return await destination().Execute(this);
        }
        public async Task<StepValue<T>> Load()
        {
            return await this.Load(() => new NothingDestination());
        }

        public abstract Task<StepValue<T>> Execute(); 
        
    }
}