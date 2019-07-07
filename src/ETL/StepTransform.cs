using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeOlho.ETL
{
    public class StepTransform<TIn, TOut> : Step<TOut> where TOut : class where TIn : class
    {
        protected readonly IStep<TIn> _stepIn;
        readonly Func<StepValue<TIn>, Task<TOut>> _transform;

        public StepTransform(IStep<TIn> stepIn, Func<StepValue<TIn>, Task<TOut>> transform)
        {
            this._stepIn = stepIn;
            this._transform = transform;
        }

        public async override Task<StepValue<TOut>> Execute()
        {
            var @in = await this._stepIn.Execute();
            var @out = await this._transform(@in);
            return  new StepValue<TOut>(@out, @in);
        }
    }
}