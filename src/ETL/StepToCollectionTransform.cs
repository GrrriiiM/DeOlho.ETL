using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeOlho.ETL
{
    public class StepToCollectionTransform<TIn, TOut> : IEnumerable<StepValue<TOut>> where TOut : class where TIn : class
    {


        readonly IEnumerator<StepValue<TOut>> _enumerator;

        public StepToCollectionTransform(Step<TIn> stepIn, Func<StepValue<TIn>, IEnumerable<TOut>> transform)
        {
            
            _enumerator = new StepEnumerator(stepIn, transform);
        }


        public IEnumerator<StepValue<TOut>> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class StepEnumerator : IEnumerator<StepValue<TOut>>
        {


            readonly Step<TIn> _stepIn;
            readonly Func<StepValue<TIn>, IEnumerable<TOut>> _transform;

            private IEnumerable<TOut> @out;
            private StepValue<TIn> @in;

            public StepEnumerator(Step<TIn> stepIn, Func<StepValue<TIn>, IEnumerable<TOut>> transform)
            {
                _stepIn = stepIn;
                _transform = transform;
            }

            public StepValue<TOut> Current 
            {
                get 
                {
                    return new StepValue<TOut>(outEnumerator.Current, @in);
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }

            private IEnumerator<TOut> outEnumerator;

            public bool MoveNext()
            {
                if (@out == null)
                {
                    @in = _stepIn.Execute().Result;
                    @out = _transform(@in);
                    outEnumerator = @out.GetEnumerator();
                }

                return outEnumerator.MoveNext();
            }

            public void Reset()
            {
                if (@out == null)
                {
                    @in = _stepIn.Execute().Result;
                    @out = _transform(@in);
                    outEnumerator = @out.GetEnumerator();
                }
                outEnumerator.Reset();
            }
        }
    }
}