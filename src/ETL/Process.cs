using System;
using System.Threading.Tasks;

namespace DeOlho.ETL
{
    public class Process
    {

        public Step<T> Extract<T>(Func<ISource<T>> source) where T : class
        {
            return new StepSource<T>(async () => await source().Execute());
        }

        public Step<T> Extract<T>(Func<Task<T>> source) where T : class
        {
            return new StepSource<T>(async () => await source());
        }
    }

}