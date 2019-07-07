using System;

namespace DeOlho.ETL
{
    public class StepValue<TValue> : IStepValue
    {

        public StepValue(TValue value, IStepValue parent)
        {
            this.Value = value;
            this.Parent = parent;
            this.TypeValue = typeof(TValue);
        }


        public TValue Value { get; private set; }
        public IStepValue Parent { get; private set; }
        public Type TypeValue { get; private set; }
        object IStepValue.Value => this.Value;

        public StepValue<TParent> GetParent<TParent>()
        {
            IStepValue stepValue = this;
            while (stepValue.Parent != null)
            {
                if (stepValue.Parent is StepValue<TParent>)
                {
                    return (StepValue<TParent>)stepValue.Parent;
                }
                stepValue = stepValue.Parent;
            }
            return null;
        }

        IStepValue IStepValue.GetParent<TParent>()
        {
            return GetParent<TParent>();
        }
    }

    public interface IStepValue
    {
        
        IStepValue Parent { get; }
        Object Value { get; }
        Type TypeValue { get; }
        IStepValue GetParent<TParent>();
    }
}