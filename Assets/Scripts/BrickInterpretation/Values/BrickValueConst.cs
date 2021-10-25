using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueConst : BrickValue
    {
        public override int Run(JArray parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject 
                && valueObject.TryGetValue("value", out int value))
            {
                return value;
            }

            throw new ArgumentException($"BrickValueConst Run has exception! Parameters {parameters}");
        }

        public override string BrickTypeName()
        {
            return "brick_value_const";
        }

        public override void Reset() { }
    }
}