using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public class BrickConditionConst : BrickCondition
    {
        public override bool Run(JArray parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject 
                && valueObject.TryGetValue("value", out bool value))
            {
                return value;
            }

            throw new ArgumentException($"BrickConditionConst Run has exception! Parameters {parameters}");
        }

        public override string BrickTypeName()
        {
            return "brick_condition_const";
        }

        public override void Reset() { }
    }
}