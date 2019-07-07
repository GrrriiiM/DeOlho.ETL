using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeOlho.ETL.Transforms
{
    public class JsonToDynamicTransform : StepTransform<string, dynamic>
    {

        public JsonToDynamicTransform(IStep<string> stepIn)
            : base(stepIn, null)
        {
        }

        public async override Task<StepValue<dynamic>> Execute()
        {
            var @in = await this._stepIn.Execute();
            var @out = (dynamic)JValue.Parse(@in.Value).ToObject<dynamic>();
            return new StepValue<dynamic>(@out, @in);
        }
    }

    public static class JsonToDynamicTransformExtensions
    {
        public static IStep<dynamic> TransformJsonToDynamic(this IStep<string> value)
        {
            return new JsonToDynamicTransform(value);
        }
        public static IEnumerable<StepValue<dynamic>> TransformJsonToDynamic(this IEnumerable<StepValue<string>> value)
        {
            return value.Select(_ => new StepValue<dynamic>(JValue.Parse(_.Value).ToObject<dynamic>(), _));
        }
    }
}