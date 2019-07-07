using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeOlho.ETL
{
    public class StepSource<TOut> : Step<TOut> where TOut : class
    {
        readonly Func<Task<TOut>> _source;

        public StepSource(Func<Task<TOut>> source)
        {
            this._source = source;
        }

        public async override Task<StepValue<TOut>> Execute()
        {
            var @out = await this._source();
            return new StepValue<TOut>(@out, null);
        }
    }
}